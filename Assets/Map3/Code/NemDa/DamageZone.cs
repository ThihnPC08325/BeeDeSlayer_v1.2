using UnityEngine;

public class DamageZone : MonoBehaviour
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private float penetration = 5f;
    [SerializeField] private float radius = 5f;

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
