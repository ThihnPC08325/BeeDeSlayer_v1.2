using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Range(0, 10)]
    [SerializeField] private float damage = 1000f; // Sát thương của viên đạn

    [SerializeField] private TrailRenderer trailRenderer; // Hiệu ứng đường đi của viên đạn

    [SerializeField] private float maxDistance = 1000f; // Khoảng cách tối đa mà viên đạn có thể bay

    [SerializeField] private float bulletLifetime = 500f; // Thời gian sống tối đa của viên đạn

    private bool IsInUse { get; set; } // Trạng thái sử dụng của viên đạn
    private float _currentDistance; // Khoảng cách hiện tại của viên đạn

    private Vector3 _startPosition; // Vị trí bắt đầu của viên đạn
    private float _maxDistanceSqr; // Bình phương khoảng cách tối đa để so sánh
    private float _spawnTime; // Thời gian sinh viên đạn

    // Mới thêm: Kích hoạt âm thanh khi viên đạn va chạm
    [SerializeField] private AudioSource collisionSound; // Âm thanh va chạm

    private void Awake()
    {
        trailRenderer = GetComponent<TrailRenderer>(); // Lấy TrailRenderer từ đối tượng
        _maxDistanceSqr = maxDistance * maxDistance; // Tính bình phương khoảng cách tối đa
    }

    private void OnEnable()
    {
        _startPosition = transform.position; // Lưu vị trí bắt đầu khi viên đạn được kích hoạt
        _spawnTime = Time.time; // Ghi lại thời gian sinh viên đạn
        IsInUse = true; // Đánh dấu viên đạn đang được sử dụng
    }

    private void OnDisable()
    {
        ResetBullet(); // Đặt lại trạng thái viên đạn khi nó không còn được sử dụng
    }

    private void Update()
    {
        if (!IsInUse) return; // Nếu viên đạn không đang sử dụng, bỏ qua

        // Kiểm tra khoảng cách
        Vector3 displacement = transform.position - _startPosition; // Tính toán khoảng cách di chuyển
        float currentDistanceSqr = displacement.sqrMagnitude; // Tính bình phương khoảng cách hiện tại

        // Cập nhật để debug
        _currentDistance = Mathf.Sqrt(currentDistanceSqr); // Tính khoảng cách hiện tại

        // Kiểm tra các điều kiện
        bool isTooFar = currentDistanceSqr >= _maxDistanceSqr; // Kiểm tra xem viên đạn có bay quá xa không
        bool timeoutReached = (Time.time - _spawnTime) >= bulletLifetime; // Kiểm tra xem thời gian sống đã hết chưa

        if (isTooFar || timeoutReached)
        {
            ReturnToPool(); // Trả viên đạn về pool nếu vượt quá khoảng cách hoặc thời gian sống
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("va cham bullet"); // Log thông báo va chạm viên đạn

        // Phát âm thanh khi va chạm (nếu có âm thanh)
        if (collisionSound != null)
        {
            collisionSound.Play(); // Phát âm thanh va chạm
        }

        switch (collision.gameObject.tag) // Kiểm tra tag của đối tượng va chạm
        {
            case "Enemy": // Nếu va chạm với kẻ thù
                HandleEnemyCollision(collision.gameObject); // Xử lý va chạm với kẻ thù
                break;
            case "Wall": // Nếu va chạm với tường
                HandleWallCollision(collision.GetContact(0).point); // Xử lý va chạm với tường
                break;
            default:
                // Xử lý các trường hợp va chạm khác nếu cần
                break;
        }

        // Biến viên đạn biến mất
        gameObject.SetActive(false); // Ẩn viên đạn trước khi trả về pool
        ReturnToPool(); // Trả viên đạn về pool sau va chạm
    }

    private void HandleEnemyCollision(GameObject enemy)
    {
        // Kích hoạt event kẻ thù trúng đạn
        GameEvents.TriggerEnemyHit(damage, enemy); // Gọi sự kiện kẻ thù bị trúng đạn

        // Xử lý sát thương
        if (enemy.TryGetComponent(out EnemyHealth enemyHealth)) // Kiểm tra và lấy EnemyHealth
        {
            enemyHealth.TakeDamage(damage); // Gây sát thương cho kẻ thù
        }

        // Kiểm tra các loại kẻ thù khác và gây sát thương tương ứng
        if (enemy.TryGetComponent(out EnemyMeleeHealth enemyMeleeHealth))
        {
            enemyMeleeHealth.TakeDamage(damage);
        }
        //Boss quai map1
        if (enemy.TryGetComponent(out GhostHealth tentacleHealth))
        {
            tentacleHealth.TakeDamage(damage);
        }
        if (enemy.TryGetComponent(out SpiderHealth spiderHealth))
        {
            spiderHealth.TakeDamage(damage);
        }
        if (enemy.TryGetComponent(out BookHealth bookHealth))
        {
            bookHealth.TakeDamage(damage);
        }
        if (enemy.TryGetComponent(out GuardianHealth guardianHealth))
        {
            guardianHealth.TakeDamage(damage);
        }
        //Boss quai map2
        if (enemy.TryGetComponent(out BatEnemyHealth batEnemyHealth))
        {
            Debug.Log("gay st bat"); // Log cho việc gây sát thương cho BatEnemy
            batEnemyHealth.TakeDamage(damage);
        }
        if (enemy.TryGetComponent(out TankerEnemyHealth tankEnemyHealth))
        {
            Debug.Log("gay st tank"); // Log cho việc gây sát thương cho TankEnemy
            tankEnemyHealth.TakeDamage(damage);
        }
        if (enemy.TryGetComponent(out BOSSHealth bossHealth))
        {
            Debug.Log("gay st boss"); // Log cho việc gây sát thương cho Boss
            bossHealth.TakeDamage(damage);
        }
        if(enemy.TryGetComponent(out BreakableCrate breakableCrate))
        {
            breakableCrate.TakeDamage(damage);
        }

        //Boss map 4
        if (enemy.TryGetComponent(out Boss4Health boss4Health))
        {
            Debug.Log("gay st boss4");
            boss4Health.TakeDamage(damage);
        }

        //Boss map 3
        if (enemy.TryGetComponent(out HealthBoss healthBossMap3))
        {
            healthBossMap3.TakeDamage(damage);
        }
        if (enemy.TryGetComponent(out LazerEnemyHealth healthLazerMap3))
        {
            healthLazerMap3.TakeDamage(damage);
        }
    }

    private void HandleWallCollision(Vector3 collisionPoint)
    {
        // Tạo hiệu ứng va chạm tại điểm va chạm
        _ = ImpactEffectPool.Instance.SpawnFromPool(ImpactEffectPool.EffectType.SoftBody, collisionPoint, Quaternion.identity);
    }

    private void ReturnToPool()
    {
        if (!BulletPool.Instance) return; // Kiểm tra nếu BulletPool không tồn tại

        ResetBullet(); // Đặt lại trạng thái viên đạn
        BulletPool.Instance.ReturnToPool(BulletPool.PoolType.NormalBullet, gameObject); // Trả viên đạn về pool
    }

    private void ResetBullet()
    {
        IsInUse = false; // Đánh dấu viên đạn không còn được sử dụng
        _startPosition = transform.position; // Đặt lại vị trí bắt đầu
        if (trailRenderer)
        {
            trailRenderer.Clear(); // Xóa trail khi reset đạn
        }
    }

    private void ReturnImpactToPool()
    {
        if (ImpactEffectPool.Instance == null) return; // Kiểm tra nếu ImpactEffectPool không tồn tại
        IsInUse = false; // Đánh dấu không sử dụng
        ImpactEffectPool.Instance.ReturnToPool(ImpactEffectPool.EffectType.SoftBody, gameObject); // Trả hiệu ứng va chạm về pool
    }
}
