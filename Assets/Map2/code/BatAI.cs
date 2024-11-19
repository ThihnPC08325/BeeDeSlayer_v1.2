using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class BatAI : MonoBehaviour
{
    private enum State { Wander, Chase }
    private State currentState;

    [SerializeField] private float wanderRadius = 10f;
    [SerializeField] private float detectionRange = 15f;
    [SerializeField] private float attackRange = 10f;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private float projectileSpeed = 3f;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float coneAngle = 15f;
    [SerializeField] private float delayBetweenShots = 0.2f;
    [SerializeField] private float attackStartDelay = 0.5f;
    private float originalSpeed;

    private NavMeshAgent agent;
    private Transform player;
    private float nextAttackTime;
    private float attackTimer;
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentState = State.Wander;
        ResetAttackTimer();

        originalSpeed = agent.speed;
    }

    private void Update()
    {
        switch (currentState)
        {
            case State.Wander:
                Wander();
                if (Vector3.Distance(transform.position, player.position) <= detectionRange)
                    currentState = State.Chase;
                break;

            case State.Chase:
                ChasePlayer();
                attackTimer -= Time.deltaTime;

                // Kiểm tra khoảng cách và thời gian để tấn công
                if (attackTimer <= 0f && Time.time >= nextAttackTime && Vector3.Distance(transform.position, player.position) <= attackRange)
                {
                    StartCoroutine(ShootProjectileCone());
                    ResetAttackTimer();
                }

                // Quay lại trạng thái Wander nếu người chơi ra khỏi tầm phát hiện
                if (Vector3.Distance(transform.position, player.position) > detectionRange)
                    currentState = State.Wander;
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

    private void ChasePlayer()
    {
        // Đặt điểm đích của agent là vị trí của người chơi
        agent.SetDestination(player.position);
    }

    private IEnumerator ShootProjectileCone()
    {
        if (animator == null || player == null) yield break;
        agent.speed = 0;
        yield return StartCoroutine(RotateTowardsPlayer(attackStartDelay));

        animator.SetBool("bat_attack", true);
        yield return new WaitForSeconds(attackStartDelay);

        for (int i = -1; i <= 1; i++)
        {
            float angleOffset = i * coneAngle;
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

        yield return new WaitForSeconds(0.1f);
        animator.SetBool("bat_attack", false);
        agent.speed = originalSpeed;
        nextAttackTime = Time.time + attackCooldown;
    }

    private IEnumerator RotateTowardsPlayer(float duration)
    {
        float timeElapsed = 0f;
        Quaternion startRotation = transform.rotation;
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));

        while (timeElapsed < duration)
        {
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        transform.rotation = targetRotation;
    }

    private void ResetAttackTimer()
    {
        attackTimer = Random.Range(3f, 6f);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, wanderRadius);
    }
}