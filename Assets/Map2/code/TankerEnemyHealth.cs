using System.Collections;
using UnityEngine;
using static EnemyManager;

public class TankerEnemyHealth : MonoBehaviour, IPooledObject
{
    [SerializeField] private float maxHealth;
    [SerializeField] private float currentHealth;
    [SerializeField] private ItemDropManager itemDropManager;
    [SerializeField] private GameObject smokePrefab; // Reference to the smoke prefab
    private BoxCollider boxCollider;
    private Animator animator;
    private TankerAI batController;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        animator = GetComponent<Animator>();
        itemDropManager = GetComponent<ItemDropManager>();
        currentHealth = maxHealth;
        batController = GetComponent<TankerAI>();
    }

    public void TakeDamage(float damage)
    {
        Debug.Log("tank nhan dame");
        currentHealth -= damage;
        if (currentHealth <= 0f)
        {
            currentHealth = 0f;
            boxCollider.enabled = false;
            Die();
        }
    }

    private void Die()
    {
        if (itemDropManager != null)
        {
            itemDropManager.TryDropLoot(transform.position);
        }

        // Instantiate the smoke effect at the enemy's position
        if (smokePrefab != null)
        {
            Instantiate(smokePrefab, transform.position, Quaternion.identity);
        }

        // Set the bat_die animation trigger
        batController.GetComponent<BatAI>();
        // Start coroutine to handle delay and deactivation
        StartCoroutine(TimeToDie(0.3f));
    }

    private IEnumerator TimeToDie(float duration)
    {
        // Wait for the duration of the death animation (2 seconds)
        yield return new WaitForSeconds(duration);

        // Deactivate the enemy GameObject
        gameObject.SetActive(false);
    }

    public virtual void OnObjectSpawn()
    {
        ResetEnemy();
    }

    protected virtual void ResetEnemy()
    {
        currentHealth = maxHealth;
        boxCollider.enabled = true;

        // Reset the bat_die animation parameter
    }
}