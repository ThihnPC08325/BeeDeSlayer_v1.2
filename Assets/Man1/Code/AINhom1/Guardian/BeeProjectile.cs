using UnityEngine;

public class BeeProjectile : MonoBehaviour
{
    [SerializeField] private float damage = 20f;
    [SerializeField] private float penetration = 5f; // Thêm độ xuyên giáp
    [SerializeField] private float lifetime = 5f;

    private void Start()
    {
        Destroy(gameObject, lifetime); // Tự hủy sau 5 giây
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Đạn của BeeWorker trúng Player!");

            // Kiểm tra PlayerHealth trước khi gọi hàm để tránh lỗi NullReferenceException
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage, penetration);
            }
            else
            {
                Debug.LogWarning("Không tìm thấy PlayerHealth trên đối tượng bị trúng đạn!");
            }

            Destroy(gameObject);
        }
    }
}
