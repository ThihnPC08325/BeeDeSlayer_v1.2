using UnityEngine;
using System.Collections;

public class BOSS : MonoBehaviour
{
    [Header("Đạn Thường")]
    [SerializeField] private float attackRange = 10f;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private float projectileSpeed = 10f;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform[] normalFirePoints;

    [Header("Kỹ Năng 1: DeadBeam")]
    [SerializeField] private float deadBeamCooldown = 20f;
    [SerializeField] private float BeamSpeed = 15f;
    [SerializeField] private GameObject deadBeamPrefab;
    [SerializeField] private float BeamDuration = 5f;
    [SerializeField] private Transform[] deadBeamFirePoints;
    [Header("Hiệu Ứng Trước Khi Bắn DeadBeam")]
    [SerializeField] private GameObject preBeamEffectPrefab;
    [SerializeField] private float preBeamEffectDuration = 2f;
    [Header("Âm thanh deadBeam")]
    [SerializeField] private AudioClip deadBeamSound;
    private AudioSource audioSource;

    [Header("Kỹ Năng 2: ném bàn")]
    [SerializeField] private float tableCooldown = 10f;
    [SerializeField] private float tableSpeed = 12f;
    [SerializeField] private GameObject tablePrefab;
    [SerializeField] private Transform[] tableFirePoints;
    [SerializeField] private float specialProjectileInitialHeight = -5f; // Độ cao xuất phát ban đầu của viên đạn

    private Transform player;
    private float nextAttackTime;
    private float nextBeamTime;
    private float nextTableTime;
    private bool isFiringBeam;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        nextAttackTime = Time.time + attackCooldown;
        nextBeamTime = Time.time + deadBeamCooldown;
        nextTableTime = Time.time + tableCooldown;
        isFiringBeam = false;

        audioSource = GetComponent<AudioSource>();

        // Kiểm tra để đảm bảo AudioSource có tồn tại
        if (audioSource == null)
        {
            Debug.LogError("AudioSource không được gắn vào BOSS.");
        }
    }

    private void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange && Time.time >= nextAttackTime && !isFiringBeam)
        {
            ShootNormalProjectile();
            nextAttackTime = Time.time + attackCooldown;
        }

        if (!isFiringBeam && Time.time >= nextBeamTime)
        {
            StartCoroutine(FireBeamAtPlayer());
        }

        if (Time.time >= nextTableTime && !isFiringBeam)
        {
            FireTable();
            nextTableTime = Time.time + tableCooldown;
        }
    }

    private void ShootNormalProjectile()
    {
        if (projectilePrefab != null && player != null)
        {
            foreach (Transform firePoint in normalFirePoints)
            {
                GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
                Rigidbody rb = projectile.GetComponent<Rigidbody>();

                if (rb != null)
                {
                    Vector3 directionToPlayer = (player.position - firePoint.position).normalized;
                    rb.velocity = directionToPlayer * projectileSpeed;
                }
            }
        }
    }

    private void FireBeamSound()
    {
        if (audioSource != null && deadBeamSound != null)
        {
            audioSource.PlayOneShot(deadBeamSound);
        }
    }
    private IEnumerator FireBeamAtPlayer()
    {
        FireBeamSound(); // Phát âm thanh trước khi bắn DeadBeam
        isFiringBeam = true;

        // Tạo hiệu ứng tại vị trí các firePoint của DeadBeam trước khi bắn
        foreach (Transform firePoint in deadBeamFirePoints)
        {
            if (preBeamEffectPrefab != null)
            {
                GameObject preEffect = Instantiate(preBeamEffectPrefab, firePoint.position, Quaternion.identity);
                Destroy(preEffect, preBeamEffectDuration); // Hủy hiệu ứng sau 2 giây
            }
        }

        // Chờ 2 giây trước khi bắn DeadBeam
        yield return new WaitForSeconds(preBeamEffectDuration);

        float timer = 0f;
        float fireInterval = 0.1f;

        // Bắt đầu bắn DeadBeam sau khi hiệu ứng kết thúc
        while (timer < BeamDuration)
        {
            foreach (Transform firePoint in deadBeamFirePoints)
            {
                if (deadBeamPrefab != null && player != null)
                {
                    GameObject beamProjectile = Instantiate(deadBeamPrefab, firePoint.position, Quaternion.identity);
                    Rigidbody rb = beamProjectile.GetComponent<Rigidbody>();

                    if (rb != null)
                    {
                        Vector3 directionToPlayer = (player.position - firePoint.position).normalized;
                        rb.velocity = directionToPlayer * BeamSpeed;
                    }
                }
            }

            timer += fireInterval;
            yield return new WaitForSeconds(fireInterval);
        }

        isFiringBeam = false;
        nextBeamTime = Time.time + deadBeamCooldown;
    }


    private void FireTable()
    {
        if (tablePrefab != null && player != null)
        {
            foreach (Transform firePoint in tableFirePoints)
            {
                StartCoroutine(DirectionPrediction(firePoint));
            }
        }
    }

    private IEnumerator DirectionPrediction(Transform firePoint)
    {
        // Vị trí bắt đầu của viên đạn (sẽ từ dưới lên trước khi nhắm vào người chơi)
        Vector3 startPos = new Vector3(firePoint.position.x, specialProjectileInitialHeight, firePoint.position.z);

        // Tạo viên đạn từ firePoint
        GameObject specialProjectile = Instantiate(tablePrefab, startPos, Quaternion.identity);
        Rigidbody rb = specialProjectile.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = false; // Đảm bảo không phải isKinematic nếu không dùng force

            // Thời gian bay lên trong 2 giây
            float riseTime = 2f;
            float startTime = Time.time;

            // Di chuyển viên đạn từ dưới lên trong 2 giây
            while (Time.time - startTime < riseTime)
            {
                // Cập nhật vị trí viên đạn bay lên
                float riseJourneyLength = riseTime * tableSpeed;
                float distanceCovered = (Time.time - startTime) * tableSpeed;
                float fractionOfJourney = distanceCovered / riseJourneyLength;
                specialProjectile.transform.position = Vector3.Lerp(startPos, firePoint.position, fractionOfJourney);

                yield return null;
            }

            // Sau khi viên đạn đã lên tới vị trí firePoint, bắt đầu tính toán hướng tới người chơi
            Vector3 targetPos = player.position;

            // Tính toán thời gian viên đạn bay đến người chơi
            float distanceToTarget = Vector3.Distance(specialProjectile.transform.position, targetPos);
            float timeToReachTarget = distanceToTarget / tableSpeed;

            // Dự đoán vị trí người chơi khi viên đạn đến (sử dụng vận tốc của người chơi)
            Vector3 playerVelocity = player.GetComponent<CharacterController>().velocity;
            Vector3 predictedTargetPos = targetPos + playerVelocity * timeToReachTarget;

            // Tính toán hướng từ vị trí của viên đạn đến vị trí dự đoán của người chơi
            Vector3 directionToPlayer = (predictedTargetPos - specialProjectile.transform.position).normalized;

            // Đặt vận tốc của viên đạn theo hướng đến người chơi
            rb.velocity = directionToPlayer * tableSpeed;
        }
        else
        {
            Debug.LogError("Không tìm thấy Rigidbody trên viên đạn!");
        }
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.blue;
        foreach (Transform firePoint in normalFirePoints)
        {
            Gizmos.DrawSphere(firePoint.position, 0.2f);
        }

        Gizmos.color = Color.green;
        foreach (Transform firePoint in tableFirePoints)
        {
            Gizmos.DrawSphere(firePoint.position, 0.2f);
        }

        Gizmos.color = Color.yellow;
        foreach (Transform firePoint in deadBeamFirePoints)
        {
            Gizmos.DrawSphere(firePoint.position, 0.2f);
        }
    }
}
