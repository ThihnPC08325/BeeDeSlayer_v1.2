using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class SmallSpider : MonoBehaviour
{
    private enum EnemyState { Idle, Patrol, Chase, Attack }
    private EnemyState currentState;

    [SerializeField] private float hopHeight = 2f;             // Height of the hop
    [SerializeField] private float hopDuration = 0.5f;         // Time taken to complete one hop
    [SerializeField] private float timeBetweenHops = 1f;       // Delay between hops
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCooldown = 2f;        // Cooldown between attacks
    [SerializeField] private float attackDelay = 0.5f;
    [SerializeField] private float attackDamage = 5f;
    [SerializeField] private float penetration = 0f;
    // Delay for attack animation

    private NavMeshAgent navMeshAgent;
    private Transform player;
    private float hopCooldown;
    private bool isHopping = false;
    private bool canAttack = true;
    private Vector3 patrolTarget;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player").transform; // Assumes the player has the "Player" tag
        currentState = EnemyState.Patrol;
        hopCooldown = timeBetweenHops;
        SetRandomPatrolPoint();
    }

    private void Update()
    {
        hopCooldown -= Time.deltaTime;

        if (!isHopping && hopCooldown <= 0)
        {
            StartCoroutine(Hop());
            hopCooldown = timeBetweenHops;
        }

        switch (currentState)
        {
            case EnemyState.Idle:
                HandleIdleState();
                break;
            case EnemyState.Patrol:
                HandlePatrolState();
                break;
            case EnemyState.Chase:
                HandleChaseState();
                break;
            case EnemyState.Attack:
                HandleAttackState();
                break;
        }
    }

    private IEnumerator Hop()
    {
        isHopping = true;
        navMeshAgent.isStopped = false;  // Enable the agent to move during the hop

        float elapsedTime = 0f;
        Vector3 originalPosition = transform.position;
        float originalY = transform.position.y;

        // Ascend phase of the hop
        while (elapsedTime < hopDuration / 2)
        {
            elapsedTime += Time.deltaTime;
            float newY = Mathf.Lerp(originalY, originalY + hopHeight, elapsedTime / (hopDuration / 2));
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
            yield return null;
        }

        elapsedTime = 0f;

        // Descend phase of the hop
        while (elapsedTime < hopDuration / 2)
        {
            elapsedTime += Time.deltaTime;
            float newY = Mathf.Lerp(originalY + hopHeight, originalY, elapsedTime / (hopDuration / 2));
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
            yield return null;
        }

        navMeshAgent.isStopped = true;  // Stop the agent after the hop
        isHopping = false;
    }

    private void HandleIdleState()
    {
        // Implement idle logic or transition to patrol
        TransitionToState(EnemyState.Patrol);
    }

    private void HandlePatrolState()
    {
        if (Vector3.Distance(transform.position, patrolTarget) < 1f)
        {
            SetRandomPatrolPoint();
        }

        if (!isHopping)
        {
            navMeshAgent.SetDestination(patrolTarget);
        }

        if (Vector3.Distance(transform.position, player.position) < detectionRange)
        {
            TransitionToState(EnemyState.Chase);
        }
    }

    private void HandleChaseState()
    {
        if (!isHopping)
        {
            navMeshAgent.SetDestination(player.position);
        }

        if (Vector3.Distance(transform.position, player.position) < attackRange && canAttack)
        {
            TransitionToState(EnemyState.Attack);
        }
        else if (Vector3.Distance(transform.position, player.position) > detectionRange)
        {
            TransitionToState(EnemyState.Patrol);
        }
    }

    private void HandleAttackState()
    {
        navMeshAgent.isStopped = true;  // Stop movement while attacking

        if (canAttack)
        {
            StartCoroutine(PerformAttack());
        }

        if (Vector3.Distance(transform.position, player.position) > attackRange)
        {
            navMeshAgent.isStopped = false;
            TransitionToState(EnemyState.Chase);
        }
    }

    private IEnumerator PerformAttack()
    {
        canAttack = false;

        // Trigger attack animation delay
        yield return new WaitForSeconds(attackDelay);

        // Apply attack effects here (e.g., damage to the player)
        PlayerHealth playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(attackDamage,penetration);
        }
        Debug.Log("Attacking Player!");

        // Wait for cooldown before allowing another attack
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;

        TransitionToState(EnemyState.Chase);  // Return to chase state after attacking
    }

    private void SetRandomPatrolPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * 10f;
        randomDirection += transform.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, 10f, NavMesh.AllAreas))
        {
            patrolTarget = hit.position;
        }
    }

    private void TransitionToState(EnemyState newState)
    {
        currentState = newState;
    }
}
