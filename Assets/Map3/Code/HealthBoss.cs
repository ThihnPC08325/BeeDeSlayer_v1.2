using System.Collections;
using UnityEngine;
using static EnemyManager;
using UnityEngine.SceneManagement;

public class HealthBoss : MonoBehaviour, IPooledObject
{
    [SerializeField] public float maxHealth;
    [SerializeField] public float currentHealth;
    [SerializeField] private ItemDropManager itemDropManager;
    [SerializeField] private GameObject smokePrefab; // Reference to the smoke prefab
    [SerializeField] private BoxCollider boxCollider;
    [SerializeField] private Boss bossController;
    [SerializeField] private HealthBarBoss healthBar;

    public static event System.Action OnBossDefeated;
    private void Awake()
    {
        itemDropManager = GetComponent<ItemDropManager>();
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        healthBar.HealthEnemy(damage);
        if (!(currentHealth <= 0f)) return;
        currentHealth = 0f;
        boxCollider.enabled = false;

        // Gọi sự kiện thông báo boss đã chết
        OnBossDefeated?.Invoke();
        Die();
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
        LevelLoader.Instance.LoadLevel(SceneManager.GetActiveScene().buildIndex + 1);
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