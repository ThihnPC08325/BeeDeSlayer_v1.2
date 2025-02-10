using UnityEngine;
using UnityEngine.AI;
using System;

public class GuardianAI : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;
    private Transform player;
    private GuardianHealth guardianHealth;

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

    // Sự kiện cái chết của Guardian
    public event Action OnGuardianDeathEvent;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        guardianHealth = GetComponent<GuardianHealth>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        navMeshAgent.speed = moveSpeed;
        MoveToNest();

        // Đăng ký sự kiện khi sức khỏe thay đổi
        guardianHealth.OnHealthChanged += HandleHealthChanged;

        // Đăng ký sự kiện khi Guardian chết
        guardianHealth.OnGuardianDeath += HandleGuardianDeath;
    }

    private void Update()
    {
        float playerDistance = Vector3.Distance(transform.position, player.position);

        if (playerDistance <= defendRadius)
        {
            EngagePlayer();
        }
        else
        {
            if (!navMeshAgent.hasPath)
            {
                MoveToNest();
            }
        }

        // Ví dụ: khi Guardian có sức khỏe dưới 50%, bắt đầu hồi phục sức khỏe
        if (guardianHealth.health <= guardianHealth.maxHealth / 2)
        {
            guardianHealth.StartHealing(1f);  // Hồi phục 5 điểm sức khỏe mỗi giây
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
        Vector3 knockbackDirection = (player.position - transform.position).normalized;
        player.GetComponent<Rigidbody>().AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);
    }

    private void MoveToNest()
    {
        navMeshAgent.SetDestination(nestPosition.position);
    }

    // Xử lý sự kiện thay đổi sức khỏe
    private void HandleHealthChanged(float currentHealth, float maxHealth)
    {
        OnHealthChangedEvent?.Invoke(currentHealth, maxHealth); // Gọi sự kiện thay đổi sức khỏe

        // Kiểm tra khi sức khỏe dưới 50% và chưa đủ số BeeWorkers
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

    // Xử lý sự kiện cái chết của Guardian
    private void HandleGuardianDeath()
    {
        OnGuardianDeathEvent?.Invoke(); // Gọi sự kiện khi Guardian chết
        Debug.Log("Guardian has been defeated!");
        Destroy(gameObject);
    }
}
