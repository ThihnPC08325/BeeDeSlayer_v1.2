using UnityEngine;

public class TentacleHead : MonoBehaviour
{
    [SerializeField] private TentacleEnemy enemyAI;
    [SerializeField] private Transform tentacleBody;
    private EnemyHealth enemyHealth;
    private Vector3 initialScale;
    private bool isRetracting = false;

    private void Awake()
    {
        if (enemyAI == null)
        {
            enemyAI = GetComponentInParent<TentacleEnemy>();
        }

        // Reference the main enemy's health component
        enemyHealth = GetComponentInParent<EnemyHealth>();

        // Store the initial scale of the tentacle to control downward extension
        initialScale = tentacleBody.localScale;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isRetracting)
        {
            enemyAI.CatchPlayer();
        }
    }

    public void TakeDamage(float damage)
    {
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(damage);  // Apply damage to main body health
        }
        Debug.Log("Bleh");
        // Trigger retraction if tentacle head takes damage
        if (!isRetracting)
        {
            Debug.Log("Bluh");
            isRetracting = true;
            enemyAI.OnTentacleHeadHit();
        }
    }

    public void ExtendTentacle(float extensionSpeed)
    {
        if (!isRetracting)
        {
            tentacleBody.localScale += new Vector3(0, extensionSpeed * Time.deltaTime, 0);
        }
    }

    public void ResetRetracting()
    {
        isRetracting = false;
        tentacleBody.localScale = initialScale;
    }
}
