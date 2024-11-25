using UnityEngine;

public class BossAttack : MonoBehaviour
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private float damagePen = 0f;

    private void Start()
    {
        Destroy(gameObject, lifetime); // Destroy the projectile after a set time
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage, damagePen);
            }
            Destroy(gameObject);
        }
    }
}
