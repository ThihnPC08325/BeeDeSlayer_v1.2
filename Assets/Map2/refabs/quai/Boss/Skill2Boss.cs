using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill2Boss : MonoBehaviour
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private float damagePen = 0f;

    // Thêm các biến để kiểm soát tốc độ xoay
    [SerializeField] private float rotationSpeedX = 10f; // Tốc độ xoay theo trục X
    [SerializeField] private float rotationSpeedY = 20f; // Tốc độ xoay theo trục Y
    [SerializeField] private float rotationSpeedZ = 30f; // Tốc độ xoay theo trục Z

    private void Start()
    {
        Destroy(gameObject, lifetime); // Hủy đối tượng sau một khoảng thời gian nhất định
    }

    private void Update()
    {
        // Cập nhật xoay đối tượng theo thời gian, với tốc độ tăng dần
        float deltaTime = Time.deltaTime; // Lấy thời gian thực hiện mỗi khung hình

        transform.Rotate(rotationSpeedX * deltaTime, rotationSpeedY * deltaTime, rotationSpeedZ * deltaTime);

        // Tăng tốc độ xoay lên sau mỗi khung hình (hoặc theo một điều kiện nào đó nếu cần)
        rotationSpeedX += 0.1f * deltaTime; // Tăng tốc độ xoay theo trục X
        rotationSpeedY += 0.1f * deltaTime; // Tăng tốc độ xoay theo trục Y
        rotationSpeedZ += 0.1f * deltaTime; // Tăng tốc độ xoay theo trục Z
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
