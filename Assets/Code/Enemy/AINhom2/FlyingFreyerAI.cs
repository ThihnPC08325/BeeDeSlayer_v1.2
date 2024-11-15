using UnityEngine.AI;
using System.Collections;
using UnityEngine;

public class FlyingFreyerAI : MonoBehaviour
{
    private enum State { Wander, Prey }
    private State currentState;

    [SerializeField] private float wanderRadius = 10f;
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float attackRange = 10f;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private float circlingDistance = 70f;
    [SerializeField] private float circlingSpeed = 20f;
    [SerializeField] private float projectileSpeed = 3f;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float coneAngle = 15f;
    [SerializeField] private float delayBetweenShots = 0.2f;

    private NavMeshAgent agent;
    private Transform player;
    private float nextAttackTime;
    private int circlingDirection = 1;
    private float attackTimer;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentState = State.Wander;
        ResetAttackTimer();
    }

    private void Update()
    {
        switch (currentState)
        {
            case State.Wander:
                Wander();
                if (Vector3.Distance(transform.position, player.position) <= detectionRange)
                    currentState = State.Prey;
                break;

            case State.Prey:
                Prey();
                attackTimer -= Time.deltaTime;
                if (attackTimer <= 0f && Time.time >= nextAttackTime && Vector3.Distance(transform.position, player.position) <= attackRange)
                {
                    StartCoroutine(ShootProjectileCone());
                    ResetAttackTimer();
                }
                break;
        }
    }

    private void Wander()
    {
        if (!agent.hasPath)
        {
            Vector3 wanderTarget = transform.position + Random.insideUnitSphere * wanderRadius;
            if (NavMesh.SamplePosition(wanderTarget, out NavMeshHit hit, wanderRadius, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }
        }
    }

    private void Prey()
    {
        // Calculate the direction to the player
        Vector3 toPlayer = (player.position - transform.position).normalized;

        // Get a perpendicular direction using Vector3.Cross to create the circling movement
        Vector3 perpendicularDir = Vector3.Cross(toPlayer, Vector3.up) * circlingDirection;

        // Calculate the target position by adding the perpendicular direction to the player's position
        Vector3 targetPosition = player.position + perpendicularDir * circlingDistance;

        // Move the agent to the target position while circling around the player
        agent.SetDestination(targetPosition);

        // Incrementally change circling direction to keep smooth motion
        circlingDirection = (Random.value > 0.995f) ? -circlingDirection : circlingDirection;
    }

    private IEnumerator ShootProjectileCone()
    {
        for (int i = -1; i <= 1; i++) // Fire 3 projectiles in a row
        {
            float angleOffset = i * coneAngle; // Calculate the offset angle
            Quaternion rotation = Quaternion.Euler(0, angleOffset, 0);
            Vector3 direction = rotation * (player.position - transform.position).normalized;

            if (projectilePrefab != null)
            {
                GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
                Rigidbody rb = projectile.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.velocity = direction * projectileSpeed;
                }
            }
            yield return new WaitForSeconds(delayBetweenShots);
        }
        nextAttackTime = Time.time + attackCooldown;
    }

    private void ResetAttackTimer()
    {
        attackTimer = Random.Range(3f, 6f);
    }
}
