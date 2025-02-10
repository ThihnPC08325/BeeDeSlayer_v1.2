using UnityEngine;
using System.Collections;

public class BOSS2 : MonoBehaviour
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

    [Header("Kỹ Năng 2: Tornado")]
    [SerializeField] private float tableCooldown = 10f;
    [SerializeField] private float tableSpeed = 12f;
    [SerializeField] private float tableDuration = 10f;
    [SerializeField] private GameObject tablePrefab;
    [SerializeField] private Transform[] tableFirePoints;

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
        FireBeamSound();
        isFiringBeam = true;

        foreach (Transform firePoint in deadBeamFirePoints)
        {
            if (preBeamEffectPrefab != null)
            {
                GameObject preEffect = Instantiate(preBeamEffectPrefab, firePoint.position, Quaternion.identity);
                Destroy(preEffect, preBeamEffectDuration);
            }
        }

        yield return new WaitForSeconds(preBeamEffectDuration);

        float timer = 0f;
        float fireInterval = 0.1f;

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
        if (tablePrefab != null && player != null && tableFirePoints.Length > 0)
        {
            GameObject tornado = Instantiate(tablePrefab, tableFirePoints[0].position, Quaternion.identity);
            StartCoroutine(TornadoChasePlayer(tornado));
        }
    }

    private IEnumerator TornadoChasePlayer(GameObject tornado)
    {
        float timer = 0f;
        while (timer < tableDuration)
        {
            if (tornado == null || player == null) yield break;

            tornado.transform.position = Vector3.MoveTowards(
                tornado.transform.position,
                player.position,
                tableSpeed * Time.deltaTime
            );

            timer += Time.deltaTime;
            yield return null;
        }

        if (tornado != null)
        {
            Destroy(tornado);
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
