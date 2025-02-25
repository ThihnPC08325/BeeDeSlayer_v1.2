using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class GhostEnemy : MonoBehaviour
{
    private enum GhostState { Wander, Chasing, Grabbing, Cooldown }
    private GhostState currentState = GhostState.Wander;

    [SerializeField] private float detectionRange = 15f;
    [SerializeField] private float grabRange = 3f;
    [SerializeField] private float shakeIntensity = 0.2f;
    [SerializeField] private float shakeDuration = 2f;
    [SerializeField] private float damagePerSecond = 10f;
    [SerializeField] private float grabCooldown = 5f;
    [SerializeField] private float chaseSpeed = 3.5f;
    [SerializeField] private float wanderSpeed = 2f;
    [SerializeField] private float wanderRadius = 10f;
    [SerializeField] private float wanderDelay = 3f;

    private Transform player;
    private PlayerHealth playerHealth;
    private PlayerController playerMovement;
    private NavMeshAgent navMeshAgent;
    private Animator animator; // Thêm Animator

    private bool isGrabbing = false;
    private Vector3 originalPlayerPosition;
    private float cooldownTimer = 0f;
    private float wanderTimer = 0f;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerHealth = player.GetComponent<PlayerHealth>();
        playerMovement = player.GetComponent<PlayerController>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>(); // Lấy Animator

        navMeshAgent.speed = wanderSpeed;
    }

    private void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        switch (currentState)
        {
            case GhostState.Wander:
                Wander();
                animator.SetBool("isRunning", false); // Idle khi lang thang
                if (distanceToPlayer <= detectionRange)
                {
                    navMeshAgent.speed = chaseSpeed;
                    currentState = GhostState.Chasing;
                }
                break;

            case GhostState.Chasing:
                animator.SetBool("isRunning", true); // Chạy khi đuổi theo
                if (distanceToPlayer <= grabRange)
                {
                    navMeshAgent.ResetPath();
                    currentState = GhostState.Grabbing;
                    StartCoroutine(GrabPlayer());
                }
                else if (distanceToPlayer > detectionRange)
                {
                    navMeshAgent.speed = wanderSpeed;
                    currentState = GhostState.Wander;
                }
                else
                {
                    navMeshAgent.SetDestination(player.position);
                }
                break;

            case GhostState.Cooldown:
                animator.SetBool("isRunning", false); // Idle khi cooldown
                cooldownTimer -= Time.deltaTime;
                if (cooldownTimer <= 0f)
                {
                    navMeshAgent.speed = wanderSpeed;
                    currentState = GhostState.Wander;
                }
                break;
        }
    }

    private void Wander()
    {
        wanderTimer -= Time.deltaTime;
        if (wanderTimer <= 0f && !navMeshAgent.hasPath)
        {
            Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
            randomDirection += transform.position;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, NavMesh.AllAreas))
            {
                navMeshAgent.SetDestination(hit.position);
            }
            wanderTimer = wanderDelay;
        }
    }

    private IEnumerator GrabPlayer()
    {
        isGrabbing = true;
        navMeshAgent.ResetPath();
        playerMovement.enabled = false;

        originalPlayerPosition = player.position;
        float elapsedTime = 0f;
        float damageInterval = 1f;
        float damageTimer = damageInterval;

        while (elapsedTime < shakeDuration)
        {
            Vector3 shakeOffset = new Vector3(
                Random.Range(-shakeIntensity, shakeIntensity),
                Random.Range(-shakeIntensity, shakeIntensity),
                0);

            player.position = originalPlayerPosition + shakeOffset;

            damageTimer -= Time.deltaTime;
            if (damageTimer <= 0f)
            {
                playerHealth.TakeDamage(damagePerSecond, 0f);
                damageTimer = damageInterval;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        player.position = originalPlayerPosition;
        playerMovement.enabled = true;

        isGrabbing = false;
        cooldownTimer = grabCooldown;
        currentState = GhostState.Cooldown;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, grabRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, wanderRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
