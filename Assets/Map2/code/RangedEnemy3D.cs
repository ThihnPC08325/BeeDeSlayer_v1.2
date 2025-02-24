using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemy3D : MonoBehaviour
{
    [Header("Combat Settings")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float shootingInterval = 2f;
    [SerializeField] private float projectileSpeed = 20f;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float detectionRange = 1000f;

    [Header("Health Settings")]
    [SerializeField] private float maxHealth;
    [SerializeField] private float currentHealth;

    [Header("Timer Settings")]
    [SerializeField] private float deathTimer = 30f;
    [SerializeField] private float currentTimer;

    [Header("Optional Settings")]
    [SerializeField] private bool showDetectionRange = true;
    [SerializeField] private LayerMask playerLayer;

    private Transform player;
    private bool canShoot = true;
    private bool isPlayerDead = false;
    private Animator animator;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentTimer = deathTimer;
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (isPlayerDead) return;

        // Đếm ngược thời gian
        currentTimer -= Time.deltaTime;
        if (currentTimer <= 0)
        {
            Die();
            return;
        }

        if (player != null)
        {
            // Xoay về phía player
            RotateTowardsPlayer();

            // Bắn khi đã nhắm đúng hướng
            if (canShoot && IsPlayerInSight())
            {
                StartCoroutine(Shoot());
            }
        }
    }

    void RotateTowardsPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    bool IsPlayerInSight()
    {
        return true; // Luôn bắn người chơi
    }

    IEnumerator Shoot()
    {
        canShoot = false;

        if (player != null)
        {
            // Tạo đạn
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

            // Lấy hướng tới người chơi
            Vector3 directionToPlayer = (player.position - firePoint.position).normalized;

            // Thêm vận tốc cho đạn
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            rb.velocity = directionToPlayer * projectileSpeed;

            // Phát âm thanh bắn (nếu có)
            if (TryGetComponent<AudioSource>(out AudioSource audioSource))
            {
                audioSource.Play();
            }
        }

        yield return new WaitForSeconds(shootingInterval);
        canShoot = true;
    }
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {

            Die();
        }
    }

    void Die()
    {
        // Thêm logic chết (hiệu ứng, điểm, hoặc hủy đối tượng)
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        if (showDetectionRange)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, detectionRange);

            if (firePoint != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawRay(firePoint.position, firePoint.forward * detectionRange);
            }
        }
    }
}