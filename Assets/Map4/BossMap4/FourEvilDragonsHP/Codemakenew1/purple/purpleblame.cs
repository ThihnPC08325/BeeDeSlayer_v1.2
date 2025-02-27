using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Purpleblame : MonoBehaviour
{
    public float speed = 20f; // Tốc độ di chuyển của đạn
    public int damageAmount = 10; // Sát thương của đạn
    public float penetration = 0f; // Sát thương xuyên thấu
    private Vector3 target; // Mục tiêu
    private Rigidbody rb; // Rigidbody của đạn

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 direction = transform.forward; // Hướng bắn ban đầu
            Debug.Log("Fire initialized with direction: " + direction);
            rb.velocity = direction * speed; // Thiết lập vận tốc ban đầu cho đạn
        }
    }

    // Phương thức để thiết lập mục tiêu cho đạn
    public void SetTarget(Vector3 targetPosition)
    {
        target = targetPosition;
        // Tính toán hướng từ điểm bắn đến mục tiêu
        Vector3 direction = (targetPosition - transform.position).normalized;
        rb.velocity = direction * speed; // Thiết lập vận tốc theo hướng mục tiêu
    }

    private void OnTriggerEnter(Collider other)
    {
        // Kiểm tra va chạm với người chơi
        if (other.CompareTag("Player"))
        {
            // Gây sát thương cho người chơi
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount, penetration);
            }
            // Hủy đạn sau khi va chạm
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Nếu va chạm với bất kỳ đối tượng nào, hủy đạn
        if (!collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}