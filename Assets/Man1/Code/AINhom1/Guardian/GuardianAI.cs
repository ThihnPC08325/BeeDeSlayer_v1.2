using UnityEngine;
using UnityEngine.AI;
using System;

public class GuardianAI : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;
    private Transform player;
    private GuardianHealth guardianHealth;
    private Animator animator;

    [Header("AI Stats")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float defendRadius = 15f;
    [SerializeField] private float attackRange = 10f;
    [SerializeField] private float knockbackForce = 5f;

    [Header("Summon Settings")]
    [SerializeField] private GameObject beeWorkerPrefab;
    [SerializeField] private Transform summonPoint;
    [SerializeField] private int maxBeeWorkers = 3;
    private int currentBeeWorkers = 0;

    [Header("Nest Position")]
    [SerializeField] private Transform nestPosition;

    // Sự kiện thay đổi sức khỏe
    public event Action<float, float> OnHealthChangedEvent;
    public event Action OnGuardianDeathEvent;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        guardianHealth = GetComponent<GuardianHealth>();
        animator = GetComponent<Animator>(); // Lấy Animator

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject) player = playerObject.transform;

        navMeshAgent.speed = moveSpeed;
        MoveToNest();

        guardianHealth.OnHealthChanged += HandleHealthChanged;
        guardianHealth.OnGuardianDeath += HandleGuardianDeath;
    }

    private void Update()
    {
        if (player == null) return;

        float playerDistance = Vector3.Distance(transform.position, player.position);
        bool isMoving = navMeshAgent.velocity.magnitude > 0.1f; // Kiểm tra có di chuyển không

        // Cập nhật Animation (Idle ↔ Run)
        animator.SetBool("isMoving", isMoving);

        if (playerDistance <= defendRadius)
        {
            EngagePlayer();
        }
        else
        {
            if (!navMeshAgent.hasPath || navMeshAgent.remainingDistance < 0.1f)
            {
                MoveToNest();
            }
        }

        if (guardianHealth.health <= guardianHealth.maxHealth / 2)
        {
            guardianHealth.StartHealing(1f);
        }
    }

    private void EngagePlayer()
    {
        navMeshAgent.SetDestination(player.position);
        if (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            KnockbackPlayer();
        }
    }

    private void KnockbackPlayer()
    {
        if (player.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            Vector3 knockbackDirection = (player.position - transform.position).normalized;
            rb.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);
        }
    }

    private void MoveToNest()
    {
        navMeshAgent.SetDestination(nestPosition.position);
    }

    private void HandleHealthChanged(float currentHealth, float maxHealth)
    {
        OnHealthChangedEvent?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= maxHealth / 2 && currentBeeWorkers < maxBeeWorkers)
        {
            SummonBeeWorker();
        }
    }

    private void SummonBeeWorker()
    {
        Instantiate(beeWorkerPrefab, summonPoint.position, Quaternion.identity);
        currentBeeWorkers++;
    }

    private void HandleGuardianDeath()
    {
        OnGuardianDeathEvent?.Invoke();
        Debug.Log("Guardian has been defeated!");
        Destroy(gameObject);
    }
}
