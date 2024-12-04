using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class FlyingBook : MonoBehaviour
{
    public enum EnemyState
    {
        Idle,
        Wandering,
        Chasing,
        Attacking,
        Sprinting,
    }

    [Header("Enemy Settings")]
    [SerializeField] private float normalSpeed = 12f;
    [SerializeField] private float sprintSpeed = 18f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float detectionRange = 20f;
    [SerializeField] private float sprintThreshold = 30f;
    [SerializeField] private float stateUpdateInterval = 0.2f;

    [Header("Wandering Settings")]
    [SerializeField] private float wanderRadius = 10f;
    [SerializeField] private float wanderWaitTimeMin = 2f;
    [SerializeField] private float wanderWaitTimeMax = 5f;

    [Header("Attack Settings")]
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private float attackDelay = 0.5f;
    [SerializeField] private float attackDamage = 5f;
    [SerializeField] private float attackPen = 0f;

    [Header("Dodge Settings")]
    [SerializeField] private bool canDodgeAbility = true;
    [SerializeField] private float dodgeDistance = 3f;
    [SerializeField] private float dodgeCooldown = 2f;
    [SerializeField] private float dodgeTime = 0.5f;
    [SerializeField] private LayerMask bulletLayer;
    [SerializeField] private float bulletDetectionRadius = 5f;

    [Header("Detection Settings")]
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask obstacleLayer;

    private NavMeshAgent agent;
    private Transform player;
    private EnemyState currentState = EnemyState.Idle;
    private BookAttack bookAttack;
    private bool isAttacking = false;
    private bool canDodge = true;
    private float lastAttackTime = 0f;
    private float sqrAttackRange;
    private float sqrDetectionRange;
    private float sqrSprintThreshold;
    private bool isDodging = false;
    private Vector3 lastDodgeDirection;
    private bool playerInSight = false;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        bookAttack = GetComponent<BookAttack>();

        // Cache squared distances for efficiency
        sqrAttackRange = attackRange * attackRange;
        sqrDetectionRange = detectionRange * detectionRange;
        sqrSprintThreshold = sprintThreshold * sprintThreshold;
    }

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        StartCoroutine(UpdateEnemyState());
    }

    private IEnumerator UpdateEnemyState()
    {
        while (true)
        {
            yield return new WaitForSeconds(stateUpdateInterval);
            TryDetectPlayer();

            float sqrDistance = player != null ? (transform.position - player.position).sqrMagnitude : Mathf.Infinity;

            if (sqrDistance <= sqrAttackRange && playerInSight && !isAttacking)
            {
                currentState = EnemyState.Attacking;
            }
            else if (sqrDistance <= sqrDetectionRange && playerInSight)
            {
                currentState = EnemyState.Chasing;
            }
            else if (sqrDistance > sqrSprintThreshold)
            {
                currentState = EnemyState.Sprinting;
            }
            else if (currentState == EnemyState.Idle || currentState == EnemyState.Wandering)
            {
                currentState = EnemyState.Wandering;
            }
        }
    }

    private void Update()
{
    if (isAttacking || isDodging)
    {
        return;
    }

    if (canDodgeAbility && DetectIncomingBullet(out Vector3 bulletDirection))
    {
        StartCoroutine(DodgeBullet(bulletDirection));
    }

    switch (currentState)
    {
        case EnemyState.Idle:
            StartCoroutine(Wander());
            break;
        case EnemyState.Wandering:
            break;
        case EnemyState.Chasing:
            if (player != null) ChasePlayer(normalSpeed);
            break;
        case EnemyState.Sprinting:
            if (player != null) ChasePlayer(sprintSpeed);
            break;
        case EnemyState.Attacking:
            // Stop the NavMeshAgent immediately
            agent.isStopped = true;
                agent.velocity = Vector3.zero;
                StartCoroutine(AttackPlayer());
            break;
    }
}


    private void Idle()
    {
        agent.SetDestination(transform.position);
        Debug.Log("Idled");
    }

    private IEnumerator Wander()
    {
        currentState = EnemyState.Wandering;
        Vector3 wanderTarget = RandomWanderTarget();
        agent.SetDestination(wanderTarget);
        agent.speed = normalSpeed;

        while (agent.remainingDistance > agent.stoppingDistance)
        {
            yield return null;
        }

        float waitTime = Random.Range(wanderWaitTimeMin, wanderWaitTimeMax);
        yield return new WaitForSeconds(waitTime);

        currentState = EnemyState.Idle;
    }

    private Vector3 RandomWanderTarget()
    {
        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
        randomDirection += transform.position;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randomDirection, out navHit, wanderRadius, -1);
        return navHit.position;
    }

    private void ChasePlayer(float speed)
    {
        if (playerInSight && player != null)
        {
            agent.SetDestination(player.position);
            agent.speed = speed;
            RotateTowardsPlayer();
        }
    }

    private void TryDetectPlayer()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRange, playerLayer);

        foreach (var collider in colliders)
        {
            Transform potentialPlayer = collider.transform;

            if (IsPlayerInSight(potentialPlayer))
            {
                player = potentialPlayer;
                playerInSight = true;
                break;
            }
        }
    }

    private bool IsPlayerInSight(Transform potentialPlayer)
    {
        Vector3 directionToPlayer = (potentialPlayer.position - transform.position).normalized;
        return !Physics.Raycast(transform.position, directionToPlayer, detectionRange, obstacleLayer);
    }

    private IEnumerator AttackPlayer()
    {
        isAttacking = true;

        // Rotate towards the player
        RotateTowardsPlayer();

        // Wait for attack animation delay
        yield return new WaitForSeconds(attackDelay);

        // Perform attack if cooldown is complete
        if (Time.time > lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;

            GameEvents.TriggerPlayerHit(attackDamage, attackPen);

            Debug.Log("Enemy attacks the player!");
        }

        // Wait for the attack cooldown
        yield return new WaitForSeconds(attackCooldown);

        // Reset attacking state and allow NavMeshAgent to move again
        isAttacking = false;
        agent.isStopped = false;
    }

    private void RotateTowardsPlayer()
    {
        if (player == null) return;

        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    private bool DetectIncomingBullet(out Vector3 bulletDirection)
    {
        bulletDirection = Vector3.zero;
        Collider[] bulletColliders = Physics.OverlapSphere(transform.position, bulletDetectionRadius, bulletLayer);

        if (bulletColliders.Length > 0)
        {
            Rigidbody bulletRb = bulletColliders[0].GetComponent<Rigidbody>();
            if (bulletRb != null)
            {
                bulletDirection = bulletRb.velocity.normalized;
                return true;
            }
        }
        return false;
    }

    private IEnumerator DodgeBullet(Vector3 bulletDirection)
    {
        if (!canDodge) yield break;

        canDodge = false;
        isDodging = true;

        Vector3 dodgeDirection = Vector3.Cross(bulletDirection, Vector3.up).normalized;

        if (Vector3.Dot(dodgeDirection, lastDodgeDirection) > 0)
        {
            dodgeDirection = -dodgeDirection;
        }

        lastDodgeDirection = dodgeDirection;

        Vector3 dodgeTarget = transform.position + dodgeDirection * dodgeDistance;
        float dodgeStartTime = Time.time;
        Vector3 startPos = transform.position;

        while (Time.time - dodgeStartTime < dodgeTime)
        {
            float t = (Time.time - dodgeStartTime) / dodgeTime;
            transform.position = Vector3.Lerp(startPos, dodgeTarget, t);
            yield return null;
        }

        yield return new WaitForSeconds(dodgeCooldown);

        isDodging = false;
        canDodge = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, sprintThreshold);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, wanderRadius);
        if (canDodgeAbility)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, bulletDetectionRadius);
        }
    }
}
