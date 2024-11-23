using System.Collections;
using UnityEngine;
using static EnemyManager;

public class BOSSHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    [SerializeField] private float currentHealth;
    [SerializeField] private GameObject smokePrefab; // Reference to the smoke prefab
    private BoxCollider boxCollider;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        currentHealth = maxHealth;
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

        // Instantiate the smoke effect at the enemy's position
        if (smokePrefab != null)
        {
            Instantiate(smokePrefab, transform.position, Quaternion.identity);
        }

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
}