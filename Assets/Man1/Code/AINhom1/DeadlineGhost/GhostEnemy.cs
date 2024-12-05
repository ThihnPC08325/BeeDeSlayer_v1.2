using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Collections;

public class GhostEnemy : MonoBehaviour
{
    private enum GhostState { Wander, Chasing, Grabbing, Cooldown }
    private GhostState currentState = GhostState.Wander;

    [SerializeField] private float detectionRange = 15f; // Range within which the ghost can detect the player
    [SerializeField] private float grabRange = 3f; // Distance within which the ghost can grab the player
    [SerializeField] private float shakeIntensity = 0.2f; // How much the player shakes
    [SerializeField] private float shakeDuration = 2f; // How long the shaking lasts
    [SerializeField] private float damagePerSecond = 10f; // Damage dealt per second while grabbing
    [SerializeField] private float grabCooldown = 5f; // Cooldown before the ghost can grab again
    [SerializeField] private float chaseSpeed = 3.5f; // Speed of the ghost when chasing
    [SerializeField] private float wanderSpeed = 2f; // Speed of the ghost when wandering
    [SerializeField] private float wanderRadius = 10f; // Radius within which the ghost can wander
    [SerializeField] private float wanderDelay = 3f; // Delay between wander movements

    private Transform player;
    private PlayerHealth playerHealth;
    private PlayerController playerMovement;
    private NavMeshAgent navMeshAgent;

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
        navMeshAgent.speed = wanderSpeed;
    }

    private void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        switch (currentState)
        {
            case GhostState.Wander:
                Wander();
                if (distanceToPlayer <= detectionRange)
                {
                    navMeshAgent.speed = chaseSpeed;
                    currentState = GhostState.Chasing;
                }
                break;

            case GhostState.Chasing:
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
        playerMovement.enabled = false; // Disable player movement
        originalPlayerPosition = player.position;

        float elapsedTime = 0f;
        float damageInterval = 1f; // Apply damage every second
        float damageTimer = damageInterval;

        while (elapsedTime < shakeDuration)
        {
            // Shake the player
            Vector3 shakeOffset = new Vector3(
                Random.Range(-shakeIntensity, shakeIntensity),
                Random.Range(-shakeIntensity, shakeIntensity),
                0);

            player.position = originalPlayerPosition + shakeOffset;

            // Deal damage over time
            damageTimer -= Time.deltaTime;
            if (damageTimer <= 0f)
            {
                playerHealth.TakeDamage(damagePerSecond, 0f);
                damageTimer = damageInterval;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Reset player position and movement
        player.position = originalPlayerPosition;
        playerMovement.enabled = true;

        isGrabbing = false;
        cooldownTimer = grabCooldown;
        currentState = GhostState.Cooldown;
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize the grab range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, grabRange);

        // Visualize the wander radius
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, wanderRadius);

        // Visualize the detection range
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
