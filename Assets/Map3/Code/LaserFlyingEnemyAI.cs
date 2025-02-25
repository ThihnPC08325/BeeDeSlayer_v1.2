using UnityEngine.AI;
using UnityEngine;
using System.Collections;

public class LaserFlyingEnemyAI : MonoBehaviour
{
    private enum AIState { Patrolling, Chasing, Attacking, Idle }
    private AIState _currentState;

    // Components
    private NavMeshAgent _agent;
    private Transform _playerTransform;
    private EnemyLaserShooter _laserShooter;
    private EnemyHealth _enemyHealth;

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
        _currentState = AIState.Patrolling;
        InitializeComponents();
        SetRandomPatrolDestination();
    }

    private void Update()
    {
        if (_enemyHealth.currentHealth <= 0) return;

        HandleAIState();
        HandleStateTransitions();
    }

    private void InitializeComponents()
    {
        _enemyHealth = GetComponent<EnemyHealth>();
        _agent = GetComponent<NavMeshAgent>();
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        _laserShooter = GetComponent<EnemyLaserShooter>();

        _agent.enabled = true;
        _agent.updatePosition = true;
        _agent.updateRotation = true;
        _agent.updateUpAxis = false;
    }

    private void HandleAIState()
    {
        switch (_currentState)
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
        if (_agent.remainingDistance < 1f)
        {
            SetRandomPatrolDestination();
        }
    }

    private void ChasePlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, _playerTransform.position);

        if (distanceToPlayer > attackRange)
        {
            _agent.SetDestination(_playerTransform.position);
        }
        else
        {
            _currentState = AIState.Attacking;
        }
    }

    private void AttackPlayer()
    {
        _agent.isStopped = true;
        transform.LookAt(_playerTransform);

        if (!IsLookingAtPlayer() || isAttacking) return;
        isAttacking = true;
        StartCoroutine(ExecuteAttackWithDelay(attackDelay));
    }

    private void HandleStateTransitions()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, _playerTransform.position);

        if (distanceToPlayer <= attackRange)
        {
            _currentState = AIState.Attacking;
        }
        else if (distanceToPlayer <= detectionRange)
        {
            _currentState = AIState.Chasing;
            _agent.speed = chaseSpeed;
        }
        else if (_currentState != AIState.Patrolling)
        {
            _currentState = AIState.Patrolling;
            SetRandomPatrolDestination();
        }
    }

    private bool IsLookingAtPlayer()
    {
        Vector3 forward = transform.forward;
        Vector3 directionToPlayer = (_playerTransform.position - transform.position).normalized;

        float dotProduct = Vector3.Dot(forward, directionToPlayer);
        float angleToPlayer = Mathf.Acos(dotProduct) * Mathf.Rad2Deg;

        return angleToPlayer <= lookAtAngleThreshold;
    }

    private void SetRandomPatrolDestination()
    {
        _agent.speed = patrolSpeed;

        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += transform.position;
        randomDirection.y = hoverHeight;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, patrolRadius, NavMesh.AllAreas))
        {
            _agent.SetDestination(hit.position);
        }
    }

    private IEnumerator ExecuteAttackWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (_currentState == AIState.Attacking)
        {
            int GachaValue = Random.Range(0, 100);
            if (GachaValue < 25)
            {
                _laserShooter.GetBombtoPlayer();
            }
            else
            {
                _laserShooter.GetFireLaserAtPlayer();
            }
        }

        if (Vector3.Distance(transform.position, _playerTransform.position) > attackRange)
        {
            StartCoroutine(SwitchStateWithDelay(AIState.Chasing, 1f));
        }

        isAttacking = false;
    }

    private IEnumerator SwitchStateWithDelay(AIState newState, float delay)
    {
        yield return new WaitForSeconds(delay);
        _currentState = newState;
        _agent.isStopped = false;
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
