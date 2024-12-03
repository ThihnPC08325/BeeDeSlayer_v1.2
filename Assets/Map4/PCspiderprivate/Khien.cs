using System.Collections;
using UnityEngine;

public class FollowEnemy : MonoBehaviour
{
    [SerializeField] private string targetObjectName = "spiderpc:Mesh"; // Tên đối tượng mà đối tượng sẽ theo
    [SerializeField] private Vector3 offset; // Khoảng cách giữa đối tượng và enemy
    [SerializeField] private float maxHealth = 100f; // Máu tối đa
    private float currentHealth; // Máu hiện tại
    private Transform target; // Biến để lưu trữ đối tượng mục tiêu

    private void Start()
    {
        // Khởi tạo sức khỏe hiện tại
        currentHealth = maxHealth;

        // Tìm đối tượng có tên là "PCspider"
        GameObject targetObject = GameObject.Find(targetObjectName);
        if (targetObject != null)
        {
            target = targetObject.transform; // Lưu trữ biến transform của đối tượng mục tiêu
        }

        // Bắt đầu coroutine để tự động trừ máu
        StartCoroutine(DeductHealthOverTime(3f, 200f));
    }

    private void Update()
    {
        if (target != null)
        {
            // Cập nhật vị trí của đối tượng để nó luôn theo sau đối tượng mục tiêu
            transform.position = target.position + offset;
        }

        // Kiểm tra xem máu có bằng 0 không
        if (currentHealth <= 0)
        {
            Destroy(gameObject); // Hủy đối tượng nếu máu về 0
        }
    }

    // Phương thức để nhận sát thương
    public void TakeDamage(float damage)
    {
        currentHealth -= damage; // Giảm máu hiện tại
        if (currentHealth < 0)
        {
            currentHealth = 0; // Đảm bảo máu không âm
        }
    }

    // Coroutine để trừ máu sau một thời gian
    private IEnumerator DeductHealthOverTime(float delay, float damage)
    {
        yield return new WaitForSeconds(delay);
        TakeDamage(damage); // Gọi phương thức để trừ máu
    }
}