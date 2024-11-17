using System.Collections;
using UnityEngine;
using static EnemyManager;

public class BatEnemyHealth : MonoBehaviour, IPooledObject
{
    [SerializeField] private float maxHealth;
    [SerializeField] private float currentHealth;
    [SerializeField] private ItemDropManager itemDropManager;
    [SerializeField] private GameObject smokePrefab; // Reference to the smoke prefab
    private BoxCollider boxCollider;
    private Animator animator;
    private BatAI batController;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        animator = GetComponent<Animator>();
        itemDropManager = GetComponent<ItemDropManager>();
        currentHealth = maxHealth;
        batController = GetComponent<BatAI>();
    }

    public void TakeDamage(float damage)
    {
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
        animator.SetBool("bat_die", true);
        batController.GetComponent<BatAI>();
        // Start coroutine to handle delay and deactivation
        StartCoroutine(TimeToDie(2f));
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
        animator.SetBool("bat_die", false);
    }
}