using UnityEngine;

public class Rocket : MonoBehaviour
{
    // Các cài đặt cho vụ nổ
    [Header("Explosion Settings")]
    [SerializeField] private float destroyDelay = 0.5f;  // Thời gian xóa model sau khi vụ nổ xảy ra (0.5 giây)
    [SerializeField] private float explosionRadius = 5f;      // Bán kính tác động của vụ nổ (5 mét)
    [SerializeField] private float explosionDamage = 50f;     // Sát thương gây ra từ vụ nổ (50 điểm sát thương)
    [SerializeField] private GameObject explosionEffectPrefab; // Prefab hiệu ứng vụ nổ (hiệu ứng Particle, ánh sáng, âm thanh, v.v.)

    // Phương thức được gọi khi tên lửa va chạm với một collider khác
    private void OnTriggerEnter(Collider other)
    {
        Explode(); // Gọi phương thức Explode khi tên lửa va chạm
        Destroy(gameObject); // Xóa tên lửa sau khi nổ
    }

    // Phương thức xử lý vụ nổ và gây sát thương
    private void Explode()
    {
        // Tạo hiệu ứng vụ nổ tại vị trí va chạm
        if (explosionEffectPrefab != null)
        {
            // Instantiate (tạo mới) hiệu ứng vụ nổ tại vị trí tên lửa va chạm
            GameObject explosionEffect = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
            // Xóa hiệu ứng vụ nổ sau thời gian destroyDelay (0.5 giây)
            Destroy(explosionEffect, destroyDelay);
        }

        // Lấy tất cả các collider trong bán kính explosionRadius (5m)
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        // Lặp qua từng collider được phát hiện trong bán kính vụ nổ
        foreach (Collider nearby in colliders)
        {
            // Kiểm tra nếu đối tượng có tag "Enemy"
            if (!nearby.CompareTag("Enemy")) continue;

            // Gửi thông điệp "TakeDamage" với số lượng sát thương (explosionDamage) cho kẻ địch
            nearby.SendMessage("TakeDamage", explosionDamage, SendMessageOptions.DontRequireReceiver);

            // Kiểm tra nếu kẻ địch có Rigidbody, áp dụng lực nổ để tạo hiệu ứng đẩy
            Rigidbody rb = nearby.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Áp dụng lực nổ lên kẻ địch có Rigidbody để tạo hiệu ứng đẩy
                rb.AddExplosionForce(explosionDamage * 10f, transform.position, explosionRadius);
            }
        }
    }

    // Hiển thị Gizmos trong Unity Editor để dễ dàng thấy bán kính vụ nổ
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow; // Đặt màu cho Gizmos
        Gizmos.DrawWireSphere(transform.position, explosionRadius); // Vẽ một vòng tròn để hiển thị bán kính vụ nổ
    }
}
