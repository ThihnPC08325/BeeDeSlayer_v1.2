using UnityEngine;

public class Rocket : MonoBehaviour
{
    [Header("Explosion Settings")]
    [SerializeField] private float destroyDelay = 0.5f;  // Thời gian xóa model sau khi nổ
    [SerializeField] private float explosionRadius = 5f;      // Bán kính tác động của vụ nổ
    [SerializeField] private float explosionDamage = 50f;     // Sát thương gây ra từ vụ nổ
    [SerializeField] private GameObject explosionEffectPrefab; // Prefab hiệu ứng vụ nổ (particle, ánh sáng, âm thanh)
    
    private void OnTriggerEnter(Collider other)
    {
        Explode();
        Destroy(gameObject);
    }
    private void Explode()
    {
        // Tạo hiệu ứng vụ nổ tại vị trí va chạm
        if (explosionEffectPrefab != null)
        {
            GameObject explosionEffect = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
            Destroy(explosionEffect, destroyDelay);
        }

        // Lấy tất cả các collider trong bán kính explosionRadius
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider nearby in colliders)
        {
            // Nếu đối tượng có tag "Enemy", áp dụng sát thương
            if (!nearby.CompareTag("Enemy")) continue;
            // Gửi thông điệp "TakeDamage" với explosionDamage
            nearby.SendMessage("TakeDamage", explosionDamage, SendMessageOptions.DontRequireReceiver);

            // Nếu enemy có Rigidbody, áp dụng lực nổ để tạo hiệu ứng đẩy
            Rigidbody rb = nearby.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionDamage * 10f, transform.position, explosionRadius);
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
