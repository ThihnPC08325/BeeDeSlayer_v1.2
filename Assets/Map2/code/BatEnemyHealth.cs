using System.Collections;
using UnityEngine;
using static EnemyManager;

public class BatEnemyHealth : MonoBehaviour, IPooledObject
{
    private static readonly int BatDie = Animator.StringToHash("bat_die");
    [SerializeField] private float maxHealth;
    [SerializeField] private float currentHealth;
    [SerializeField] private ItemDropManager itemDropManager;
    [SerializeField] private GameObject smokePrefab; // Reference to the smoke prefab
    private BoxCollider _boxCollider;
    private Animator _animator;
    private BatAI _batController;

    private void Awake()
    {
        _boxCollider = GetComponent<BoxCollider>();
        _animator = GetComponent<Animator>();
        itemDropManager = GetComponent<ItemDropManager>();
        currentHealth = maxHealth;
        _batController = GetComponent<BatAI>();
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (!(currentHealth <= 0f)) return;
        currentHealth = 0f;
        _boxCollider.enabled = false;
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
        _animator.SetBool(BatDie, true);
        _batController.GetComponent<BatAI>();
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
        _boxCollider.enabled = true;

        // Reset the bat_die animation parameter
        _animator.SetBool(BatDie, false);
    }
}