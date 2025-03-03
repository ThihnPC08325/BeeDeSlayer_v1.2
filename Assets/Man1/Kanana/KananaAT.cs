using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class KananaAT : MonoBehaviour
{
    // Khai báo các biến để cấu hình thanh kiếm
    [SerializeField] private int damage = 30; // Sát thương mà kiếm gây ra khi tấn công
    [SerializeField] private float attackCooldown = 0.5f; // Thời gian giữa các đòn đánh (cooldown)
    private bool canAttack = true; // Kiểm tra nếu có thể tấn công (ngăn ngừa spam tấn công)
    [SerializeField] private Animator animator; // Animator để điều khiển hoạt ảnh vung kiếm

    // Phương thức Update được gọi mỗi frame
    private void Update()
    {
        // Kiểm tra nếu người chơi nhấn chuột trái (button 0)
        if (Input.GetMouseButtonDown(0) && canAttack) // 0 là chuột trái
        {
            // Nếu có thể tấn công, bắt đầu coroutine Attack
            StartCoroutine(Attack());
        }
    }

    // Coroutine Attack để thực hiện đòn tấn công với cooldown
    private IEnumerator Attack()
    {
        canAttack = false; // Đặt canAttack = false để ngừng tấn công khi đang trong cooldown

        // Kích hoạt hoạt ảnh vung kiếm trong Animator
        if (animator)
        {
            animator.SetTrigger("KananaAnimation"); // Gọi trigger "KananaAnimation" để thực hiện animation vung kiếm
        }

        // Kiểm tra va chạm với đối tượng (kẻ địch) bằng Raycast
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 2f)) // Phóng tia từ vị trí hiện tại của kiếm, theo hướng forward
        {
            // Kiểm tra nếu đối tượng va chạm có tag "Enemy"
            if (hit.collider.CompareTag("Enemy")) // Nếu trúng đối tượng có tag "Enemy"
            {
                // Gọi phương thức TakeDamage trên đối tượng để gây sát thương
                hit.collider.SendMessage("TakeDamage", damage); // Gửi sát thương cho đối tượng va chạm
                Debug.Log("Trúng mục tiêu!"); // In ra thông báo khi trúng mục tiêu
            }
        }

        // Đợi cho đến khi cooldown kết thúc
        yield return new WaitForSeconds(attackCooldown); // Đợi thời gian cooldown trước khi có thể tấn công lại
        canAttack = true; // Sau khi cooldown kết thúc, cho phép tấn công lần nữa
    }
}
