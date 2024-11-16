using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class TankerAI : MonoBehaviour
{
    private enum State { Chase, Attack, SpecialAttack }
    private State currentState;

    [SerializeField] private Transform player;
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float specialAttackRange = 1.5f;
    [SerializeField] private float normalAttackCooldown = 2f;
    [SerializeField] private float specialAttackCooldown = 5f;
    [SerializeField] private float attackDelay = 0.5f;
    [SerializeField] private float postSpecialAttackDelay = 1f;
    [SerializeField] private float normalAttackDamage = 10f;
    [SerializeField] private float normalPen = 0f;
    [SerializeField] private float specialAttackDamage = 25f;
    [SerializeField] private float specialPen = 5f;

    private NavMeshAgent agent;
    private Animator animator;
    private float normalAttackCooldownTimer;
    private float specialAttackCooldownTimer;
    private bool isAttacking;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        normalAttackCooldownTimer = 0f;
        specialAttackCooldownTimer = 0f;
        isAttacking = false;
    }

    private void Update()
    {
        if (isAttacking) return; // Bỏ qua logic nếu đang tấn công

        normalAttackCooldownTimer -= Time.deltaTime;
        specialAttackCooldownTimer -= Time.deltaTime;

        switch (currentState)
        {
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

    private void ChaseState()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Dừng di chuyển nếu trong phạm vi tấn công
        if (distanceToPlayer <= attackRange)
        {
            agent.ResetPath(); // Dừng di chuyển

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
            agent.SetDestination(player.position); // Tiếp tục di chuyển đến vị trí người chơi
        }
    }

    private IEnumerator AttackState()
    {
        isAttacking = true;
        agent.ResetPath(); // Dừng di chuyển để thực hiện tấn công
        animator.SetTrigger("TriggerAttack"); // Kích hoạt animation tấn công thường
        yield return new WaitForSeconds(attackDelay); // Chờ một thời gian để hoàn thành animation

        // Gây sát thương tấn công thường
        PlayerHealth playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(normalAttackDamage, normalPen);
        }
        Debug.Log("Performing normal attack with damage: " + normalAttackDamage);

        normalAttackCooldownTimer = normalAttackCooldown; // Đặt lại thời gian hồi chiêu

        isAttacking = false;
        currentState = State.Chase;
    }

    private IEnumerator SpecialAttackState()
    {
        isAttacking = true;
        agent.ResetPath(); // Dừng di chuyển để thực hiện tấn công đặc biệt
        animator.SetTrigger("TriggerSpecialAttack"); // Kích hoạt animation tấn công đặc biệt
        yield return new WaitForSeconds(attackDelay); // Chờ một thời gian để hoàn thành animation

        // Gây sát thương tấn công đặc biệt
        PlayerHealth playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(specialAttackDamage, specialPen);
        }
        Debug.Log("Performing special attack with damage: " + specialAttackDamage);

        // Chờ thêm thời gian sau khi hoàn tất tấn công đặc biệt
        yield return new WaitForSeconds(postSpecialAttackDelay);

        specialAttackCooldownTimer = specialAttackCooldown; // Đặt lại thời gian hồi chiêu
        normalAttackCooldownTimer = normalAttackCooldown; // Đặt lại thời gian hồi chiêu cho tấn công thường

        isAttacking = false;
        currentState = State.Chase;
    }
}