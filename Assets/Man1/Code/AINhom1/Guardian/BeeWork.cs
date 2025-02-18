using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BeeWork : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;
    private Transform player;
    private Animator animator;

    [Header("AI Stats")]
    [SerializeField] private float patrolSpeed = 3f;
    [SerializeField] private float patrolRadius = 30f;
    [SerializeField] private float flyingHeight = 10f;

    [Header("Detection and Shooting")]
    [SerializeField] private float detectionRange = 20f;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private float projectileSpeed = 5f;

    private float lastAttackTime;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        navMeshAgent.enabled = true;
        navMeshAgent.updatePosition = true;
        navMeshAgent.updateRotation = true;
        navMeshAgent.updateUpAxis = false;
        navMeshAgent.speed = patrolSpeed;

        animator.SetBool("isWalking", false);
        SetRandomPatrolDestination();
    }

    private void Update()
    {
        // Nếu AI có vận tốc thì chạy animation đi bộ
        animator.SetBool("isWalking", navMeshAgent.velocity.magnitude > 0.1f);

        // Nếu tới điểm đến, chọn điểm mới
        if (navMeshAgent.remainingDistance < 1f || !navMeshAgent.hasPath)
        {
            SetRandomPatrolDestination();
        }

        // Kiểm tra khoảng cách với player
        float playerDistance = Vector3.Distance(transform.position, player.position);
        if (playerDistance <= detectionRange)
        {
            TryShootAtPlayer();
        }
    }

    private void SetRandomPatrolDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += transform.position;
        randomDirection.y = flyingHeight;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, NavMesh.AllAreas))
        {
            navMeshAgent.SetDestination(hit.position);
        }
    }

    private void TryShootAtPlayer()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            StartCoroutine(ShootProjectile());
            lastAttackTime = Time.time;
        }
    }

    private IEnumerator ShootProjectile()
    {
        animator.SetTrigger("isAttacking"); // Kích hoạt animation tấn công
        yield return new WaitForSeconds(0.3f); // Chờ một chút trước khi bắn

        Vector3 directionToPlayer = (player.position - projectileSpawnPoint.position).normalized;
        GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
        BrokenflyProjectile projectileScript = projectile.GetComponent<BrokenflyProjectile>();
        if (projectileScript != null)
        {
            projectileScript.SetDirection(directionToPlayer);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, patrolRadius);

        Gizmos.color = Color.cyan;
        Vector3 groundPosition = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 flyingPosition = new Vector3(transform.position.x, flyingHeight, transform.position.z);
        Gizmos.DrawLine(groundPosition, flyingPosition);
    }
}
