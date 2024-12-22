using UnityEngine;
using System.Collections;

public class BossController : MonoBehaviour
{
    public GameObject projectilePrefab; // Prefab của viên đạn
    public float projectileSpeed = 100f; // Tốc độ của viên đạn
    public float shootCooldown = 5f;   // Thời gian hồi chiêu giữa mỗi đợt bắn
    public float projectileInterval = 5f; // Thời gian giữa các viên đạn
    public int numberOfProjectiles = 5; // Số viên đạn mỗi đợt bắn
    public int projectileDamage = 5;    // Sát thương mỗi viên đạn
    public LayerMask playerLayer;       // Layer của Player

    public GameObject bombPrefab; // Prefab bom
    public float spawnRadius = 20f; // Bán kính spawn bom xung quanh vị trí người chơi
    public float explosionDelay = 3f; // Thời gian chờ để bom nổ
    public float damageRadius = 5f;  // Bán kính vụ nổ
    public int bombDamage = 10;      // Sát thương bom
    public GameObject explosionEffectPrefab; // Particle Effect khi bom nổ

    private Transform player; // Vị trí người chơi
    private bool isShooting = false;  // Kiểm tra xem Boss có đang bắn không
    public Transform projectileSpawnPoint; // Điểm xuất phát viên đạn (cần đặt trong Unity)

    void Start()
    {
        // Tìm đối tượng người chơi
        player = GameObject.FindWithTag("Player")?.transform;

        // Kiểm tra nếu Player không được tìm thấy
        if (player == null)
        {
            Debug.LogWarning("Player not found! Make sure the player has the correct tag.");
        }
        else
        {
            // Kích hoạt hành vi bắn định kỳ
            InvokeRepeating("ShootProjectiles", 2f, shootCooldown);

            // Kích hoạt spawn bom ngẫu nhiên
            InvokeRepeating("SpawnBombs", 2f, 5f); // Spawn bom mỗi 5 giây sau 2 giây delay
        }
    }

    void Update()
    {
        // Kiểm tra nếu Boss đang hoạt động trước khi thực thi bất kỳ hành động nào
        if (!gameObject.activeInHierarchy) return;

        // Kiểm tra nếu player tồn tại và boss ở trong phạm vi gần
        if (player != null && !isShooting)
        {
            ShootProjectiles();
        }
    }

    void ShootProjectiles()
    {
        // Kiểm tra nếu không có đợt bắn nào đang diễn ra và Boss còn hoạt động
        if (player != null && !isShooting && gameObject.activeInHierarchy)
        {
            // Bắt đầu Coroutine để bắn liên tiếp từng viên
            StartCoroutine(ShootSequence());
        }
    }

    private IEnumerator ShootSequence()
    {
        // Đánh dấu Boss đang bắn
        isShooting = true;

        for (int i = 0; i < numberOfProjectiles; i++)
        {
            // Kiểm tra nếu Boss còn hoạt động
            if (!gameObject.activeInHierarchy) yield break;

            // Lấy vị trí hiện tại của người chơi
            Vector3 targetPosition = player.position;

            // Tạo viên đạn tại vị trí spawnPoint thay vì transform.position của Boss
            Vector3 spawnPosition = projectileSpawnPoint.position;
            GameObject projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);

            // Gán sát thương cho viên đạn
            ProjectileController pc = projectile.GetComponent<ProjectileController>();
            if (pc != null)
            {
                pc.damage = projectileDamage;

                // Gán hướng di chuyển và tốc độ cho viên đạn
                Rigidbody rb = projectile.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    // Tính toán hướng di chuyển từ điểm spawn đến người chơi
                    Vector3 direction = (targetPosition - spawnPosition).normalized;
                    rb.velocity = direction * projectileSpeed;
                }
            }

            // Chờ một khoảng thời gian trước khi bắn viên tiếp theo
            yield return new WaitForSeconds(projectileInterval);
        }

        // Đánh dấu Boss đã bắn xong
        isShooting = false;
    }

    void SpawnBombs()
    {
        // Kiểm tra nếu Boss còn hoạt động
        if (!gameObject.activeInHierarchy) return;

        if (player != null)
        {
            // Spawn 3 quả bom
            for (int i = 0; i < 3; i++)
            {
                // Tạo ra vị trí spawn ngẫu nhiên gần người chơi
                Vector3 randomPosition = player.position + new Vector3(
                    Random.Range(-spawnRadius, spawnRadius), // Random x
                    0f, // Y giữ nguyên
                    Random.Range(-spawnRadius, spawnRadius)  // Random z
                );

                // Spawn bom tại vị trí ngẫu nhiên
                GameObject bomb = Instantiate(bombPrefab, randomPosition, Quaternion.identity);

                // Tăng kích thước của bom
                bomb.transform.localScale = new Vector3(5f, 5f, 5f);

                // Gọi hàm Explode sau khi bom có thời gian trì hoãn
                StartCoroutine(ExplodeBomb(bomb));
            }
        }
    }

    private IEnumerator ExplodeBomb(GameObject bomb)
    {
        // Kiểm tra nếu Boss còn hoạt động
        if (!gameObject.activeInHierarchy) yield break;

        // Chờ trước khi bom nổ
        yield return new WaitForSeconds(explosionDelay);

        // Kiểm tra nếu Boss còn hoạt động
        if (!gameObject.activeInHierarchy) yield break;

        // Tạo hiệu ứng nổ tại vị trí bom
        Instantiate(explosionEffectPrefab, bomb.transform.position, Quaternion.identity);

        // Kiểm tra va chạm với Player trong bán kính vụ nổ
        Collider[] hitPlayers = Physics.OverlapSphere(bomb.transform.position, damageRadius, playerLayer);

        foreach (Collider hit in hitPlayers)
        {
            // Gọi hàm Explode từ PlayerHealth để gây sát thương
            hit.GetComponent<PlayerHealth>()?.TakeDamage(bombDamage, 0);
        }

        // Xóa bom sau khi nổ
        Destroy(bomb);
    }

    void OnBossDeath()
    {
        // Ngừng các hành động như bắn và spawn bom khi Boss chết
        CancelInvoke("ShootProjectiles");
        CancelInvoke("SpawnBombs");
        StopAllCoroutines(); // Ngừng tất cả các Coroutine đang chạy
    }

    private void OnDrawGizmosSelected()
    {
        // Hiển thị bán kính spawn và bán kính nổ trong Editor
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, damageRadius);
    }
}
