using UnityEngine;
using UnityEngine.AI;

public class BrokenFlyAI : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;
    private Transform player;

    [Header("AI Stats")]
    [SerializeField] private float patrolSpeed = 3f;
    [SerializeField] private float patrolRadius = 30f;
    [SerializeField] private float flyingHeight = 10f;

    [Header("Detection and Shooting")]
    [SerializeField] private float detectionRange = 20f;
    [SerializeField] private float attackCooldown = 2f; // Time between attacks
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private float projectileSpeed = 5f; // Projectile speed
    [SerializeField] private Animator animator;

    private float lastAttackTime;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        navMeshAgent.enabled = true;
        navMeshAgent.updatePosition = true;
        navMeshAgent.updateRotation = true;
        navMeshAgent.updateUpAxis = false; // Flying, so we control the up axis.
        navMeshAgent.speed = patrolSpeed;

        SetRandomPatrolDestination();
    }

    private void Update()
    {
        // Patrol and continuously move
        if (navMeshAgent.remainingDistance < 1f || !navMeshAgent.hasPath)
        {
            SetRandomPatrolDestination();
        }

        // Check if the player is within detection range
        float playerDistance = Vector3.Distance(transform.position, player.position);
        if (playerDistance <= detectionRange)
        {
            animator.SetTrigger("Fly_Atk");
            TryShootAtPlayer();
        }
    }

    private void SetRandomPatrolDestination()
    {
        // Generate a random point within the patrol radius
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += transform.position;
        randomDirection.y = flyingHeight; // Maintain flying height

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, NavMesh.AllAreas))
        {
            navMeshAgent.SetDestination(hit.position);
        }
    }

    private void TryShootAtPlayer()
    {
        // Check attack cooldown
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            ShootProjectile();
            lastAttackTime = Time.time;
        }
    }

    private void ShootProjectile()
    {
        // Calculate direction toward the player
        Vector3 directionToPlayer = (player.position - projectileSpawnPoint.position).normalized;

        // Instantiate the projectile and set its direction
        GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
        BrokenflyProjectile projectileScript = projectile.GetComponent<BrokenflyProjectile>();
        if (projectileScript != null)
        {
            projectileScript.SetDirection(directionToPlayer);
        }
    }

    // Optional: Draw detection range and patrol radius for debugging
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
