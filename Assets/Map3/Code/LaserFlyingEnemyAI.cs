using UnityEngine.AI;
using UnityEngine;
using System.Collections;

public class LaserFlyingEnemyAI : MonoBehaviour
{
    private enum AIState { Patrolling, Chasing, Attacking, Idle }
    private AIState currentState;

    // Components
    private NavMeshAgent agent;
    private Transform playerTransform;
    private EnemyLaserShooter laserShooter;
    private EnemyHealth enemyHealth;

    // AI Stats
    [Header("AI Stats")]
    [SerializeField] private float detectionRange = 20f;
    [SerializeField] private float attackRange = 15f;
    [SerializeField] private float patrolSpeed = 3f;
    [SerializeField] private float chaseSpeed = 6f;
    [SerializeField] private float patrolRadius = 30f;
    [SerializeField] private float hoverHeight = 10f;
    [SerializeField] private float attackDelay = 1.0f;
    [SerializeField] private float lookAtAngleThreshold = 5f;

    // State Control
    private bool isAttacking = false;

    private void Start()
    {
        currentState = AIState.Patrolling;
        InitializeComponents();
        SetRandomPatrolDestination();
    }

    private void Update()
    {
        if (enemyHealth.currentHealth <= 0) return;

        HandleAIState();
        HandleStateTransitions();
    }

    private void InitializeComponents()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        agent = GetComponent<NavMeshAgent>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        laserShooter = GetComponent<EnemyLaserShooter>();

        agent.enabled = true;
        agent.updatePosition = true;
        agent.updateRotation = true;
        agent.updateUpAxis = false;
    }

    private void HandleAIState()
    {
        switch (currentState)
        {
            case AIState.Patrolling:
                Patrol();
                break;
            case AIState.Chasing:
                ChasePlayer();
                break;
            case AIState.Attacking:
                AttackPlayer();
                break;
            case AIState.Idle:
                Idle();
                break;
        }
    }

    private void Idle()
    {
        // Do nothing in Idle state
    }

    private void Patrol()
    {
        if (agent.remainingDistance < 1f)
        {
            SetRandomPatrolDestination();
        }
    }

    private void ChasePlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer > attackRange)
        {
            agent.SetDestination(playerTransform.position);
        }
        else
        {
            currentState = AIState.Attacking;
        }
    }

    private void AttackPlayer()
    {
        agent.isStopped = true;
        transform.LookAt(playerTransform);

        if (IsLookingAtPlayer() && !isAttacking)
        {
            isAttacking = true;
            StartCoroutine(ExecuteAttackWithDelay(attackDelay));
        }
    }

    private void HandleStateTransitions()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= attackRange)
        {
            currentState = AIState.Attacking;
        }
        else if (distanceToPlayer <= detectionRange)
        {
            currentState = AIState.Chasing;
            agent.speed = chaseSpeed;
        }
        else if (currentState != AIState.Patrolling)
        {
            currentState = AIState.Patrolling;
            SetRandomPatrolDestination();
        }
    }

    private bool IsLookingAtPlayer()
    {
        Vector3 forward = transform.forward;
        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;

        float dotProduct = Vector3.Dot(forward, directionToPlayer);
        float angleToPlayer = Mathf.Acos(dotProduct) * Mathf.Rad2Deg;

        return angleToPlayer <= lookAtAngleThreshold;
    }

    private void SetRandomPatrolDestination()
    {
        agent.speed = patrolSpeed;

        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += transform.position;
        randomDirection.y = hoverHeight;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, patrolRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    private IEnumerator ExecuteAttackWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (currentState == AIState.Attacking)
        {
            int GachaValue = Random.Range(0, 100);
            if (GachaValue < 25)
            {
                laserShooter.GetBombtoPlayer();
            }
            else
            {
                laserShooter.GetFireLaserAtPlayer();
            }
        }

        if (Vector3.Distance(transform.position, playerTransform.position) > attackRange)
        {
            StartCoroutine(SwitchStateWithDelay(AIState.Chasing, 1f));
        }

        isAttacking = false;
    }

    private IEnumerator SwitchStateWithDelay(AIState newState, float delay)
    {
        yield return new WaitForSeconds(delay);
        currentState = newState;
        agent.isStopped = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, patrolRadius);

        Gizmos.color = Color.cyan;
        Vector3 groundPosition = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 hoverPosition = new Vector3(transform.position.x, hoverHeight, transform.position.z);
        Gizmos.DrawLine(groundPosition, hoverPosition);
    }
}
