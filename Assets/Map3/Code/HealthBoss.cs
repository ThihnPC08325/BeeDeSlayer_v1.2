using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyManager;

public class HealthBoss : MonoBehaviour, IPooledObject
{
    [SerializeField] public float maxHealth;
    [SerializeField] public float currentHealth;
    [SerializeField] private ItemDropManager itemDropManager;
    [SerializeField] private GameObject smokePrefab; // Reference to the smoke prefab
    [SerializeField] private BoxCollider boxCollider;
    [SerializeField] private Boss bossController;
    [SerializeField] HealthBarBoss healthBar;
    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        healthBar.HealthEnemy(damage);
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
        bossController.GetComponent<BatAI>();
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
