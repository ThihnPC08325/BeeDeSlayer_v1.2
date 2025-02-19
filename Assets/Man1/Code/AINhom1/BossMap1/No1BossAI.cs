using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Collections;

public class No1BossAI : MonoBehaviour
{
    // State Enum
    private enum BossState { Chasing, MeleeAttacking, RangedAttacking, Enraged }

    [Header("References")]
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Animator animator;

    [Header("Settings")]
    [SerializeField] private float meleeRange = 3f;
    [SerializeField] private float meleeDamage = 20f;
    [SerializeField] private float meleeAttackCooldown = 1.5f;
    [SerializeField] private float meleeAttackDelay = 0.5f; // Delay before dealing damage for animation
    [SerializeField] private float penetration = 0f;

    [SerializeField] private float rangedCooldown = 5f;
    [SerializeField] private float rangedAttackDelay = 1f; // Delay before firing the projectile
    [SerializeField] private float enrageDistance = 10f;
    [SerializeField] private int enragedShots = 3;

    private NavMeshAgent agent;
    private Transform player;
    private BossState currentState = BossState.Chasing;
    private float lastRangedAttackTime;
    private float lastMeleeAttackTime;
    private int enragedShotsFired;
    private bool isMeleeAttackOnCooldown;
    private bool isEnragedShooting = false;

    // Animation

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError("Player not found! Make sure the Player GameObject has the 'Player' tag.");
        }
    }

    private void Start()
    {
        EnterState(BossState.Chasing); // Immediately chase the player
    }

    private void Update()
    {
        if (player == null) return; // Exit if player is not assigned
        HandleState();
    }

    private void HandleState()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        switch (currentState)
        {
            case BossState.Chasing:
                agent.isStopped = false; // Resume movement
                agent.SetDestination(player.position);

                if (distanceToPlayer <= meleeRange && !isMeleeAttackOnCooldown)
                {
                    EnterState(BossState.MeleeAttacking);
                }
                else if (distanceToPlayer > enrageDistance)
                {
                    EnterState(BossState.Enraged);
                }
                else if (Time.time - lastRangedAttackTime >= rangedCooldown)
                {
                    EnterState(BossState.RangedAttacking);
                }
                break;

            case BossState.MeleeAttacking:
                agent.isStopped = true;

                if (Time.time - lastMeleeAttackTime >= meleeAttackCooldown)
                {
                    PerformMeleeAttack();
                }
                else if (distanceToPlayer > meleeRange)
                {
                    EnterState(BossState.Chasing);
                }
                break;

            case BossState.RangedAttacking:
                agent.isStopped = true;
                if (Time.time - lastRangedAttackTime >= rangedCooldown)
                {
                    PerformRangedAttack();
                }
                break;

            case BossState.Enraged:
                agent.isStopped = true;

                if (enragedShotsFired < enragedShots)
                {
                    PerformEnragedShot();
                }
                else
                {
                    // Reset the shot count and return to Chasing
                    enragedShotsFired = 0;
                    EnterState(BossState.Chasing);
                }
                break;
        }
    }

    private void EnterState(BossState newState)
    {
        currentState = newState;

        switch (newState)
        {
            case BossState.Chasing:
                agent.isStopped = false;
                animator.SetBool("rangeAttack", false);
                animator.SetBool("meleeAttack", false);
                break;

            case BossState.MeleeAttacking:
                agent.isStopped = true;
                animator.SetBool("meleeAttack", true);
                break;

            case BossState.RangedAttacking:
                agent.isStopped = true;
                animator.SetBool("rangeAttack", true);

                // Reset ranged attack timer
                lastRangedAttackTime = Time.time;

                // Perform the ranged attack
                PerformRangedAttack();
                break;

            case BossState.Enraged:
                agent.isStopped = true;
                animator.SetBool("rangeAttack", true);
                break;
        }
    }

    private void PerformMeleeAttack()
    {
        lastMeleeAttackTime = Time.time;
        isMeleeAttackOnCooldown = true;
        Invoke(nameof(DealMeleeDamage), meleeAttackDelay);
        Invoke(nameof(ResetMeleeCooldown), meleeAttackCooldown);
    }

    private void DealMeleeDamage()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= meleeRange && player != null)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(meleeDamage, penetration);
            }
        }
    }

    private void ResetMeleeCooldown()
    {
        isMeleeAttackOnCooldown = false;
    }

    private void PerformRangedAttack()
    {
        if (player == null) return;


        // Delay the actual attack to allow for animations
        Invoke(nameof(ExecuteRangedAttack), rangedAttackDelay);
    }

    private void ExecuteRangedAttack()
    {
        if (player == null) return;


        // Instantiate the projectile but do not set its direction yet
        GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);

        // Wait at the spawn point for a moment before firing
        StartCoroutine(WaitAndFireProjectile(projectile));
    }

    private IEnumerator WaitAndFireProjectile(GameObject projectile)
    {
        float waitDuration = 0.6f; // Adjust this to change the delay
        yield return new WaitForSeconds(waitDuration);


        if (player == null || projectile == null) yield break;

        // Aim and fire the projectile toward the player
        Vector3 direction = (player.position - projectileSpawnPoint.position).normalized;
        Boss1Projectile projectileScript = projectile.GetComponent<Boss1Projectile>();
        if (projectileScript != null)
        {
            projectileScript.SetDirection(direction);
        }
        else
        {
            Debug.LogError("Projectile prefab must have a Projectile script.");
        }

        EnterState(BossState.Chasing);
    }

    private void PerformEnragedShot()
    {
        if (IsInvoking(nameof(FireEnragedShot))) return; // Prevent overlapping invokes

        Debug.Log($"Enraged Shot {enragedShotsFired + 1} of {enragedShots}");

        Invoke(nameof(FireEnragedShot), rangedAttackDelay);
    }

    private void FireEnragedShot()
    {
        if (player == null) return;


        GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);

        StartCoroutine(WaitAndFireProjectile(projectile));

        enragedShotsFired++;

        if (enragedShotsFired < enragedShots)
        {
            PerformEnragedShot();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, enrageDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, meleeRange);
    }
}
