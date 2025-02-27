using UnityEngine;

public class DamageZone : MonoBehaviour
{
    public float damage = 10f;
    public float penetration = 5f;
    public float radius = 5f;

    void Start()
    {
        ApplyDamageInRadius();
    }

    void ApplyDamageInRadius()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider hitCollider in hitColliders)
        {
            PlayerHealth character = hitCollider.GetComponent<PlayerHealth>();
            if (character != null)
            {
                character.TakeDamage(damage, penetration);
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
