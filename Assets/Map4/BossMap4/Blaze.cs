using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class laserGame : MonoBehaviour
{
    public Transform laserOrigin; // Vị trí laser
    public float gunRange = 800f; // Phạm vi bắn
    public float fireRate = 5f; // Tốc độ bắn
    public float laserDuration = 0.5f; // Thời gian hiển thị laser
    public string targetTag = "Player"; // Tag của đối tượng cần bắn
    public GameObject explosionEffectPrefab;  // Particle Effect khi viên đạn phát nổ
    public AudioClip shootSound; // Âm thanh bắn
    private AudioSource audioSource; // Tham chiếu đến AudioSource

    private LineRenderer laserLine;
    private float fireTimer;

    void Awake()
    {
        laserLine = GetComponent<LineRenderer>();
        audioSource = gameObject.AddComponent<AudioSource>(); // Thêm AudioSource
    }

    void Update()
    {
        fireTimer += Time.deltaTime;

        // Nếu đã đến thời gian bắn tiếp theo
        if (fireTimer >= fireRate)
        {
            fireTimer = 0; // Reset thời gian
            CheckAndFire(); // Kiểm tra và bắn nếu Player trong phạm vi
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Phát nổ và hủy viên đạn sau khi va chạm
        Explode();
    }

    void Explode()
    {
        // Tạo hiệu ứng nổ tại vị trí viên đạn
        Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);

        // Hủy viên đạn sau khi phát nổ
        Destroy(gameObject);
    }

    void CheckAndFire()
    {
        // Tìm tất cả các đối tượng trong phạm vi
        Collider[] hits = Physics.OverlapSphere(laserOrigin.position, gunRange);

        foreach (Collider hit in hits)
        {
            // Kiểm tra nếu đối tượng có tag là Player
            if (hit.CompareTag(targetTag))
            {
                FireLaser(hit.transform.position); // Gọi hàm bắn laser
                break; // Ngừng sau khi bắn Player đầu tiên
            }
        }
    }

    void FireLaser(Vector3 targetPosition)
    {
        Debug.Log("Firing laser...");
        laserLine.SetPosition(0, laserOrigin.position);
        laserLine.SetPosition(1, targetPosition);

        // Phát âm thanh khi bắn
        audioSource.PlayOneShot(shootSound);

        RaycastHit hit;
        if (Physics.Raycast(laserOrigin.position, (targetPosition - laserOrigin.position).normalized, out hit, gunRange))
        {
            Debug.Log($"Hit: {hit.transform.name}");
            PlayerHealth health = hit.transform.GetComponent<PlayerHealth>();
            if (health != null)
            {
                Debug.Log("Player hit! Applying damage...");
                health.TakeDamage(10f, 0f);
            }
        }

        StartCoroutine(ShootLaser());
    }

    IEnumerator ShootLaser()
    {
        laserLine.enabled = true; // Bật laser
        yield return new WaitForSeconds(laserDuration); // Chờ một chút
        laserLine.enabled = false; // Tắt laser
    }

    void OnDrawGizmosSelected()
    {
        // Vẽ vùng phạm vi bắn để dễ kiểm tra trong Scene View
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(laserOrigin.position, gunRange);
    }
}