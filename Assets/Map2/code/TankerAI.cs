using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class TankerAI : MonoBehaviour
{
    private enum State
    {
        Chase,
        Attack,
        SpecialAttack
    }

    private State _currentState;

    [Header("Target & Ranges")] [SerializeField]
    private float detectionRange = 10f;

    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float specialAttackRange = 1.5f;

    [Header("Attack Settings")] [SerializeField]
    private float normalAttackCooldown = 2f;

    [SerializeField] private float specialAttackCooldown = 5f;
    [SerializeField] private float attackDelay = 0.5f;
    [SerializeField] private float postSpecialAttackDelay = 1f;

    [Header("Damage Settings")] [SerializeField]
    private float normalAttackDamage = 10f;

    [SerializeField] private float normalPen = 0f;
    [SerializeField] private float specialAttackDamage = 25f;
    [SerializeField] private float specialPen = 5f;

    private NavMeshAgent _agent;
    private Animator _animator;
    private float _normalAttackCooldownTimer;
    private float _specialAttackCooldownTimer;
    private bool _isAttacking;
    private bool _canMove = true;
    private Transform _player;

    private static readonly int IsMoving = Animator.StringToHash("isMoving");
    private static readonly int TriggerAttack = Animator.StringToHash("TriggerAttack");
    private static readonly int TriggerSpecialAttack = Animator.StringToHash("TriggerSpecialAttack");

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _normalAttackCooldownTimer = 0f;
        _specialAttackCooldownTimer = 0f;
        _isAttacking = false;
        _canMove = true;
    }

    private void Update()
    {
        if (_isAttacking || !_canMove) return;

        _normalAttackCooldownTimer -= Time.deltaTime;
        _specialAttackCooldownTimer -= Time.deltaTime;

        switch (_currentState)
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

        // Update animation
        _animator.SetBool(IsMoving, _agent.velocity.magnitude > 0.1f);
    }

    private void ChaseState()
    {
        if (!_canMove) return;

        float distanceToPlayer = Vector3.Distance(transform.position, _player.position);

        // Look at player
        Vector3 direction = (_player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

        if (distanceToPlayer <= attackRange)
        {
            _agent.ResetPath();
            _animator.SetBool(IsMoving, false);

            if (distanceToPlayer <= specialAttackRange && _specialAttackCooldownTimer <= 0f)
            {
                _currentState = State.SpecialAttack;
            }
            else if (_normalAttackCooldownTimer <= 0f)
            {
                _currentState = State.Attack;
            }
        }
        else
        {
            _agent.SetDestination(_player.position);
            _animator.SetBool(IsMoving, true);
        }
    }

    private IEnumerator AttackState()
    {
        _isAttacking = true;
        _canMove = false;
        _agent.ResetPath();
        _animator.SetBool(IsMoving, false);
        _animator.SetTrigger(TriggerAttack);

        // Lấy độ dài của animation hiện tại
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        float animationLength = stateInfo.length;

        yield return new WaitForSeconds(attackDelay);

        // Gây sát thương
        GameEvents.TriggerPlayerHit(normalAttackDamage, normalPen);

        // Chờ animation kết thúc
        yield return new WaitForSeconds(animationLength - attackDelay);

        _normalAttackCooldownTimer = normalAttackCooldown;
        _isAttacking = false;
        _canMove = true;
        _currentState = State.Chase;
    }

    private IEnumerator SpecialAttackState()
    {
        _isAttacking = true;
        _canMove = false;
        _agent.ResetPath();
        _animator.SetBool(IsMoving, false);
        _animator.SetTrigger(TriggerSpecialAttack);

        // Lấy độ dài của animation hiện tại
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        float animationLength = stateInfo.length;

        yield return new WaitForSeconds(attackDelay);

        // Gây sát thương
        PlayerHealth playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
        if (playerHealth)
        {
            playerHealth.TakeDamage(specialAttackDamage, specialPen);
        }

        // Chờ animation kết thúc + thời gian delay sau special attack
        yield return new WaitForSeconds((animationLength - attackDelay) + postSpecialAttackDelay);

        _specialAttackCooldownTimer = specialAttackCooldown;
        _normalAttackCooldownTimer = normalAttackCooldown;
        _isAttacking = false;
        _canMove = true;
        _currentState = State.Chase;
    }

    // Debug helpers
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, specialAttackRange);
    }
}