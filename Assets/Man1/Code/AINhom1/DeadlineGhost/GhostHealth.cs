using System;
using System.Collections;
using UnityEngine;

public class GhostHealth : MonoBehaviour
{
    public float health = 100f;
    public event Action OnEnemyDefeated;

    [SerializeField] private ItemDropManager itemDropManager;

    private BoxCollider BoxCollider;

    private void Awake()
    {
        BoxCollider = GetComponent<BoxCollider>();
        itemDropManager = GetComponent<ItemDropManager>();
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0f)
        {
            health = 0f;
            BoxCollider.enabled = false;
            Die();
        }
    }

    private void Die()
    {
        if (itemDropManager != null)
        {
            itemDropManager.TryDropLoot(transform.position);
        }
        gameObject.SetActive(false);
        OnEnemyDefeated?.Invoke();
        StartCoroutine(TimeToDie(5f));
    }

    private IEnumerator TimeToDie(float duration)
    {
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }
}