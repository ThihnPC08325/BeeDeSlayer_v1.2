using UnityEngine;
using UnityEngine.AI;

public class BeeWorkerAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform player;

    [SerializeField] private float attackRange = 5f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float speed = 3f;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent.speed = speed;
    }

    private void Update()
    {
        agent.SetDestination(player.position);
        if (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            AttackPlayer();
        }
    }

    private void AttackPlayer()
    {
        Debug.Log("Bee Worker tấn công người chơi!");
    }
}
