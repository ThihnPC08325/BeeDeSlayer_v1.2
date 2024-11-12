using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Range(0, 100)]
    [SerializeField] private float damage = 10f;

    [SerializeField] private TrailRenderer trailRenderer;

    [SerializeField] private float maxDistance = 100f;
    [SerializeField] private float bulletLifetime = 5f;

    private bool IsInUse { get; set; }
    private float currentDistance;

    private Vector3 startPosition;
    private float maxDistanceSqr;
    private float spawnTime;

    private void Awake()
    {
        trailRenderer = GetComponent<TrailRenderer>();
        maxDistanceSqr = maxDistance * maxDistance;
    }

    private void OnEnable()
    {
        startPosition = transform.position;
        spawnTime = Time.time;
        IsInUse = true;
    }

    private void OnDisable()
    {
        ResetBullet();
    }

    private void Update()
    {
        if (!IsInUse) return;

        // Kiểm tra khoảng cách
        Vector3 displacement = transform.position - startPosition;
        float currentDistanceSqr = displacement.sqrMagnitude;

        // Cập nhật để debug
        currentDistance = Mathf.Sqrt(currentDistanceSqr);

        // Kiểm tra multiple conditions
        bool isTooFar = currentDistanceSqr >= maxDistanceSqr;
        bool timeoutReached = (Time.time - spawnTime) >= bulletLifetime;

        if (isTooFar || timeoutReached)
        {
            ReturnToPool();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Enemy":
                HandleEnemyCollision(collision.gameObject);
                break;
            case "Wall":
                HandleWallCollision(collision.GetContact(0).point);
                break;
            default:
                // Xử lý các trường hợp va chạm khác nếu cần
                break;
        }

        ReturnToPool();
        ReturnImpactToPool();
    }

    private void HandleEnemyCollision(GameObject enemy)
    {
        // Kích hoạt event kẻ thù trúng đạn
        GameEvents.TriggerEnemyHit(damage, enemy);

        // Xử lý sát thương
        if (enemy.TryGetComponent(out EnemyHealth enemyHealth))
        {
            enemyHealth.TakeDamage(damage);
        }

        if (enemy.TryGetComponent(out EnemyMeleeHealth enemyMeleeHealth))
        {
            enemyMeleeHealth.TakeDamage(damage);
        }
    }

    private void HandleWallCollision(Vector3 collisionPoint)
    {
        _ = ImpactEffectPool.Instance.SpawnFromPool("ImpactSoftBody", collisionPoint, Quaternion.identity);
    }

    private void ReturnToPool()
    {
        if (BulletPool.Instance == null) return;

        ResetBullet();
        BulletPool.Instance.ReturnToPool("Bullet", gameObject);
    }

    private void ResetBullet()
    {
        IsInUse = false;
        startPosition = transform.position; // Đặt lại vị trí bắt đầu
        if (trailRenderer != null)
        {
            trailRenderer.Clear(); // Xóa trail khi reset đạn
        }
    }

    private void ReturnImpactToPool()
    {
        if (ImpactEffectPool.Instance == null) return;
        IsInUse = false;
        ImpactEffectPool.Instance.ReturnToPool("ImpactSoftBody", gameObject);

    }
}