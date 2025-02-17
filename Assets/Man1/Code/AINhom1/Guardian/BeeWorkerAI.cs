using UnityEngine;
using UnityEngine.AI;

public class BeeWorkerAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform player;

    [Header("Stats")]
    [SerializeField] private float maxHealth = 50f;
    private float currentHealth;

    [SerializeField] private float attackRange = 10f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float speed = 3f;

    [Header("Shooting")]
    [SerializeField] private GameObject projectilePrefab; // Prefab đạn
    [SerializeField] private Transform projectileSpawnPoint; // Vị trí bắn đạn
    [SerializeField] private float projectileSpeed = 10f;
    [SerializeField] private float fireRate = 2f; // Thời gian giữa các lần bắn
    [SerializeField] private float rotationSpeed = 5f; // Tốc độ quay mặt về Player

    private float nextFireTime = 0f; // Biến lưu thời gian cho lần bắn tiếp theo

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent.speed = speed;
        currentHealth = maxHealth;
    }

    private void Update()
    {
        agent.SetDestination(player.position);

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            RotateTowardsPlayer(); // Quay mặt về Player

            if (Time.time >= nextFireTime)
            {
                ShootProjectile();
                nextFireTime = Time.time + fireRate; // Cập nhật thời gian bắn tiếp theo
            }
        }
    }

    private void RotateTowardsPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0; // Giữ nguyên trục Y để không bị nghiêng
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    private void ShootProjectile()
    {
        if (projectilePrefab != null && projectileSpawnPoint != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
            Rigidbody rb = projectile.GetComponent<Rigidbody>();

            if (rb != null)
            {
                Vector3 directionToPlayer = (player.position - projectileSpawnPoint.position).normalized;
                rb.velocity = directionToPlayer * projectileSpeed;
            }

            Debug.Log("BeeWorker bắn đạn!");
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        Debug.Log($"Bee Worker bị tấn công! Máu còn: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Bee Worker đã chết!");
        Destroy(gameObject);
    }
}
