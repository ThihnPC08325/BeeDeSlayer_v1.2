using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedHeal : MonoBehaviour
{
    public int HP = 100; // Điểm sức khỏe của rồng
    public Animator animator; // Animator để điều khiển hoạt ảnh

    private void OnTriggerEnter(Collider other)
    {
        // Kiểm tra xem đối tượng va chạm có phải là "Bullet" không
        if (other.CompareTag("Bullet"))
        {
            // Gọi phương thức TakeDamage với giá trị sát thương cố định là 10
            TakeDamage(10);

            // Hủy bullet sau khi va chạm
            Destroy(other.gameObject);
        }
    }

    public void TakeDamage(int damageAmount)
    {
        HP -= damageAmount; // Giảm HP với sát thương nhận được
        if (HP <= 0)
        {
            animator.SetTrigger("die"); // Kích hoạt hoạt ảnh chết
            GetComponent<Collider>().enabled = false; // Vô hiệu hóa collider của rồng
        }
        else
        {
            animator.SetTrigger("damage"); // Kích hoạt hoạt ảnh bị thương
        }
    }
}
