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
    private Transform _player;
    private PlayerHealth _playerHealth;

    private float _normalAttackTimer;
    private float _specialAttackTimer;
    private bool _isAttacking;

    private State _currentState;

    private static readonly int IsMovingHash = Animator.StringToHash("isMoving");
    private static readonly int TriggerAttackHash = Animator.StringToHash("TriggerAttack");
    private static readonly int TriggerSpecialAttackHash = Animator.StringToHash("TriggerSpecialAttack");

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        _player = playerObj.transform;
        _playerHealth = playerObj.GetComponent<PlayerHealth>();
    }

    private void Update()
    {
        UpdateCooldowns();

        if (_isAttacking) return;

        float distanceToPlayer = Vector3.Distance(transform.position, _player.position);
        if (distanceToPlayer <= specialAttackRange && _specialAttackTimer <= 0f)
        {
            ChangeState(State.SpecialAttack);
        }
        else if (distanceToPlayer <= attackRange && _normalAttackTimer <= 0f)
        {
            ChangeState(State.Attack);
        }
        else
        {
            ChangeState(State.Chase);
        }

        HandleState(distanceToPlayer);

        UpdateAnimations();
    }

    private void UpdateCooldowns()
    {
        _normalAttackTimer = Mathf.Max(_normalAttackTimer - Time.deltaTime, 0);
        _specialAttackTimer = Mathf.Max(_specialAttackTimer - Time.deltaTime, 0);
    }

    private void ChangeState(State newState)
    {
        if (Equals(_currentState, newState)) return;
        _currentState = newState;
    }

    private void HandleState(float distanceToPlayer)
    {
        switch (_currentState)
        {
            case State.Chase:
                ChasePlayer(distanceToPlayer);
                break;
            case State.Attack:
                StartCoroutine(PerformAttack());
                break;
            case State.SpecialAttack:
                StartCoroutine(PerformSpecialAttack());
                break;
        }
    }

    private void ChasePlayer(float distanceToPlayer)
    {
        if (distanceToPlayer > attackRange)
        {
            _agent.SetDestination(_player.position);
            RotateTowardsPlayer();
        }
        else
        {
            if (!_agent.isStopped) _agent.ResetPath();
        }
    }

    private void RotateTowardsPlayer()
    {
        Vector3 direction = (_player.position - transform.position).normalized;
        direction.y = 0f;
        Quaternion rotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 5f);
    }

    private IEnumerator PerformAttack()
    {
        _isAttacking = true;
        _agent.ResetPath();
        _animator.SetTrigger(TriggerAttackHash);

        yield return new WaitForSeconds(attackDelay);
        // Trigger damage
        GameEvents.TriggerPlayerHit(normalAttackDamage, normalPen);

        yield return new WaitForSeconds(normalAttackCooldown);
        _normalAttackTimer = normalAttackCooldown;

        _isAttacking = false;
    }

    private IEnumerator PerformSpecialAttack()
    {
        _isAttacking = true;
        _agent.ResetPath();
        _animator.SetTrigger(TriggerSpecialAttackHash);

        yield return new WaitForSeconds(attackDelay);
        // Directly apply special damage
        if (_playerHealth)
            _playerHealth.TakeDamage(specialAttackDamage, specialPen);

        yield return new WaitForSeconds(postSpecialAttackDelay);
        _specialAttackTimer = specialAttackCooldown;
        _normalAttackTimer = normalAttackCooldown; // reset cả normal attack timer nếu cần

        _isAttacking = false;
    }

    private void UpdateAnimations()
    {
        bool isMoving = _agent.velocity.magnitude > 0.1f && !_isAttacking;
        _animator.SetBool(IsMovingHash, isMoving);
    }

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