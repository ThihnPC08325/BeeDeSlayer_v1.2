using UnityEngine;

public class BombController : MonoBehaviour
{
    public GameObject explosionEffectPrefab; // Particle Effect khi bom nổ
    public float damageRadius = 5f;  // Bán kính vụ nổ
    public int damage = 20;           // Sát thương
    public LayerMask playerLayer;     // Layer của Player

    void Explode()
    {
        // Tạo hiệu ứng nổ tại vị trí bom
        Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);

        // Kiểm tra va chạm với Player trong bán kính vụ nổ
        Collider[] hitPlayers = Physics.OverlapSphere(transform.position, damageRadius, playerLayer);

        foreach (Collider player in hitPlayers)
        {
            // Gọi hàm TakeDamage từ PlayerHealth nếu Player đứng trong vùng nổ
            player.GetComponent<PlayerHealth>()?.TakeDamage(damage, 0);
        }

        // Xóa bom sau khi nổ
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        // Hiển thị bán kính vụ nổ trong Editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, damageRadius);
    }
}
