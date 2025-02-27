using UnityEngine;

public class DamageTrigger : MonoBehaviour
{
    public int damageAmount = 50; // Số điểm sát thương gây ra
    public float penetration = 0f; // Giá trị xuyên thấu (nếu cần)

    private void OnTriggerEnter(Collider other)
    {
        // Kiểm tra xem đối tượng va chạm có phải là người chơi không
        if (other.CompareTag("Player"))
        {
            // Gọi phương thức để gây sát thương cho người chơi
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount, penetration);
            }
        }
    }
}