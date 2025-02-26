using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class BatAI : MonoBehaviour
{
    private static readonly int Attack = Animator.StringToHash("bat_attack");

    private enum State { Wander, Chase }
    private State _currentState;

    [SerializeField] private float wanderRadius = 10f;
    [SerializeField] private float detectionRange = 15f;
    [SerializeField] private float attackRange = 10f;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private float projectileSpeed = 3f;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float coneAngle = 15f;
    [SerializeField] private float delayBetweenShots = 0.2f;
    [SerializeField] private float attackStartDelay = 0.5f;
    private float _originalSpeed;

    private NavMeshAgent _agent;
    private Transform _player;
    private float _nextAttackTime;
    private float _attackTimer;
    private Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _currentState = State.Wander;
        ResetAttackTimer();

        _originalSpeed = _agent.speed;
    }

    private void Update()
    {
        switch (_currentState)
        {
            case State.Wander:
                Wander();
                if (Vector3.Distance(transform.position, _player.position) <= detectionRange)
                    _currentState = State.Chase;
                break;

            case State.Chase:
                ChasePlayer();
                _attackTimer -= Time.deltaTime;

                // Kiểm tra khoảng cách và thời gian để tấn công
                if (_attackTimer <= 0f && Time.time >= _nextAttackTime && Vector3.Distance(transform.position, _player.position) <= attackRange)
                {
                    StartCoroutine(ShootProjectileCone());
                    ResetAttackTimer();
                }

                // Quay lại trạng thái Wander nếu người chơi ra khỏi tầm phát hiện
                if (Vector3.Distance(transform.position, _player.position) > detectionRange)
                    _currentState = State.Wander;
                break;
        }
    }

    private void Wander()
    {
        if (_agent.hasPath) return;
        Vector3 wanderTarget = transform.position + Random.insideUnitSphere * wanderRadius;
        if (NavMesh.SamplePosition(wanderTarget, out NavMeshHit hit, wanderRadius, NavMesh.AllAreas))
        {
            _agent.SetDestination(hit.position);
        }
    }

    private void ChasePlayer()
    {
        // Đặt điểm đích của agent là vị trí của người chơi
        _agent.SetDestination(_player.position);
    }

    private IEnumerator ShootProjectileCone()
    {
        if (!_animator || !_player) yield break;
        _agent.speed = 0;
        yield return StartCoroutine(RotateTowardsPlayer(attackStartDelay));

        _animator.SetBool(Attack, true);
        yield return new WaitForSeconds(attackStartDelay);

        for (int i = -1; i <= 1; i++)
        {
            float angleOffset = i * coneAngle;
            Quaternion rotation = Quaternion.Euler(0, angleOffset, 0);
            Vector3 direction = rotation * (_player.position - transform.position).normalized;

            if (projectilePrefab)
            {
                GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
                Rigidbody rb = projectile.GetComponent<Rigidbody>();

                if (rb)
                {
                    rb.velocity = direction * projectileSpeed;
                }
            }

            yield return new WaitForSeconds(delayBetweenShots);
        }

        yield return new WaitForSeconds(0.1f);
        _animator.SetBool(Attack, false);
        _agent.speed = _originalSpeed;
        _nextAttackTime = Time.time + attackCooldown;
    }

    private IEnumerator RotateTowardsPlayer(float duration)
    {
        float timeElapsed = 0f;
        Quaternion startRotation = transform.rotation;
        Vector3 directionToPlayer = (_player.position - transform.position).normalized;
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
        _attackTimer = Random.Range(3f, 6f);
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