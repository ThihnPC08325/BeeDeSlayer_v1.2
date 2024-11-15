using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class TankerEnemy : MonoBehaviour
{
    private enum State { Idle, Chase, Attack, SpecialAttack }
    private State currentState;

    [SerializeField] private Transform player;
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float specialAttackRange = 1.5f;
    [SerializeField] private float normalAttackCooldown = 2f;
    [SerializeField] private float specialAttackCooldown = 5f;
    [SerializeField] private float attackDelay = 0.5f; // Delay for animation timing
    [SerializeField] private float postSpecialAttackDelay = 1f; // Delay after special attack
    [SerializeField] private float normalAttackDamage = 10f;
    [SerializeField] private float normalPen = 0f;
    [SerializeField] private float specialAttackDamage = 25f;
    [SerializeField] private float specialPen = 5f;

    private NavMeshAgent agent;
    private float normalAttackCooldownTimer;
    private float specialAttackCooldownTimer;
    private bool isAttacking;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        currentState = State.Idle;
        normalAttackCooldownTimer = 0f;
        specialAttackCooldownTimer = 0f;
        isAttacking = false;
    }

    private void Update()
    {
        if (isAttacking) return; // Skip Update logic if already performing an attack

        normalAttackCooldownTimer -= Time.deltaTime;
        specialAttackCooldownTimer -= Time.deltaTime;

        switch (currentState)
        {
            case State.Idle:
                IdleState();
                break;
            case State.Chase:
                ChaseState();
                break;
            case State.Attack:
                StartCoroutine(AttackState());
                break;
            case State.SpecialAttack:
                StartCoroutine(SpecialAttackState());
                break;
        }
    }

    private void IdleState()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= detectionRange)
        {
            currentState = State.Chase;
        }
    }

    private void ChaseState()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Stop moving if within attack range but in cooldown
        if (distanceToPlayer <= attackRange)
        {
            agent.ResetPath(); // Stop movement

            if (distanceToPlayer <= specialAttackRange && specialAttackCooldownTimer <= 0f)
            {
                currentState = State.SpecialAttack;
            }
            else if (normalAttackCooldownTimer <= 0f)
            {
                currentState = State.Attack;
            }
        }
        else
        {
            agent.SetDestination(player.position); // Continue chasing if out of range
        }

        // Return to Idle if player is out of detection range
        if (distanceToPlayer > detectionRange)
        {
            currentState = State.Idle;
        }
    }

    private IEnumerator AttackState()
    {
        isAttacking = true;
        agent.ResetPath(); // Stop moving to perform the attack
        yield return new WaitForSeconds(attackDelay); // Wait for animation

        // Perform normal attack
        PlayerHealth playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(normalAttackDamage,normalPen);
        }
        Debug.Log("Performing normal attack with damage: " + normalAttackDamage);
        normalAttackCooldownTimer = normalAttackCooldown; // Reset cooldown

        isAttacking = false;
        currentState = State.Chase;
    }

    private IEnumerator SpecialAttackState()
    {
        isAttacking = true;
        agent.ResetPath(); // Stop moving to perform the special attack
        yield return new WaitForSeconds(attackDelay); // Wait for animation

        // Perform special attack
        PlayerHealth playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(specialAttackDamage,specialPen);
        }
        Debug.Log("Performing special attack with damage: " + specialAttackDamage);

        // Apply a delay after the special attack
        yield return new WaitForSeconds(postSpecialAttackDelay);

        // Reset both cooldowns after the special attack
        specialAttackCooldownTimer = specialAttackCooldown;
        normalAttackCooldownTimer = normalAttackCooldown;

        isAttacking = false;
        currentState = State.Chase;
    }
}
