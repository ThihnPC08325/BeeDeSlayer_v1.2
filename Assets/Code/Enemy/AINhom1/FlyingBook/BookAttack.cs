using UnityEngine;

public class BookAttack : MonoBehaviour
{
    public float damage = 10f;
    public float damagePen = 0f;
    public void TryAttack()
    {
        // Assuming player has a health script
        PlayerHealth playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage, damagePen);
        }
    }
}
