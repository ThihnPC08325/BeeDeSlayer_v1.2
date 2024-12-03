using UnityEngine;
using System.Collections;

public class BossController : MonoBehaviour
{
    public GameObject projectilePrefab; // Prefab của viên đạn
    public float projectileSpeed = 50f; // Tốc độ của viên đạn
    public float shootCooldown = 20f;   // Thời gian hồi chiêu giữa mỗi đợt bắn
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
    public float detectionRange = 30f; // Khoảng cách phát hiện người chơi

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
        // Kiểm tra khoảng cách giữa boss và người chơi
        if (player != null && Vector3.Distance(transform.position, player.position) <= detectionRange)
        {
            // Nếu người chơi nằm trong khoảng cách phát hiện, thực hiện hành động
            if (!isShooting)
            {
                ShootProjectiles();
            }
        }
    }

    void ShootProjectiles()
    {
        // Kiểm tra nếu không có đợt bắn nào đang diễn ra
        if (player != null && !isShooting)
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
            // Lấy vị trí hiện tại của người chơi
            Vector3 targetPosition = player.position;

            // Tạo viên đạn tại vị trí của Boss
            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

            // Gán sát thương cho viên đạn
            ProjectileController pc = projectile.GetComponent<ProjectileController>();
            if (pc != null)
            {
                pc.damage = projectileDamage;
                // Gán hướng di chuyển và tốc độ cho viên đạn
                Rigidbody rb = projectile.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Vector3 direction = (targetPosition - transform.position).normalized;
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
        if (player != null && Vector3.Distance(transform.position, player.position) <= detectionRange)
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

    // Coroutine xử lý nổ bom sau một khoảng thời gian delay
    private IEnumerator ExplodeBomb(GameObject bomb)
    {
        // Chờ trước khi bom nổ
        yield return new WaitForSeconds(explosionDelay);

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

    private void OnDrawGizmosSelected()
    {
        // Hiển thị bán kính spawn và bán kính nổ trong Editor
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, damageRadius);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange); // Hiển thị khoảng cách phát hiện
    }
}