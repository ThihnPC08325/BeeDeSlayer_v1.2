using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyManager;

public class LazerEnemyHealth : MonoBehaviour, IPooledObject
{
    [SerializeField] private float maxHealth;
    [SerializeField] public float currentHealth;
    [SerializeField] private ItemDropManager itemDropManager;
    [SerializeField] private EnemyController enemyController;
    [SerializeField] private HealthBarLazer healthBar; //Map 3
    private BoxCollider _boxCollider;

    private void Awake()
    {
        _boxCollider = GetComponent<BoxCollider>();
        itemDropManager = GetComponent<ItemDropManager>();
        enemyController = GetComponent<EnemyController>();
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        healthBar.HealthEnemy(damage);
        if (currentHealth <= 0f)
        {
            currentHealth = 0f;
            _boxCollider.enabled = false;
            enemyController.enabled = false;
            Die();
        }
    }

    private void Die()
    {
        if (itemDropManager != null)
        {
            itemDropManager.TryDropLoot(transform.position);
        }
        StartCoroutine(TimeToDie(5f));
        gameObject.SetActive(false);
    }

    private static IEnumerator TimeToDie(float duration)
    {
        yield return new WaitForSeconds(duration);
    }

    public virtual void OnObjectSpawn()
    {
        ResetEnemy();
    }

    protected virtual void ResetEnemy()
    {
        currentHealth = maxHealth;
        _boxCollider.enabled = true;
        enemyController.enabled = true;
    }
}
