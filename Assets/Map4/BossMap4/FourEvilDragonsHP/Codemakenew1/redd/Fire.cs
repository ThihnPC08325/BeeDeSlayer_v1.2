using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    public GameObject firePrefab; // Prefab của viên đạn
    public float speed = 10f; // Tốc độ di chuyển của đạn
    public int damageAmount = 10; // Sát thương của đạn
    public float penetration = 0f; // Sát thương xuyên thấu
    private Vector3 target; // Mục tiêu
    public float maxDistance = 10f; // Khoảng cách tối đa mà viên đạn có thể bay
    public float lifetime = 1f; // Thời gian tồn tại của viên đạn

    private Vector3 initialPosition; // Vị trí ban đầu của viên đạn

    private void Start()
    {
        // Lưu vị trí ban đầu
        initialPosition = transform.position;
    }

    // Phương thức để thiết lập mục tiêu cho đạn
    public void SetTarget(Vector3 targetPosition)
    {
        target = targetPosition;
        StartCoroutine(MoveToTarget());
    }

    private IEnumerator MoveToTarget()
    {
        // Tạo viên đạn tại vị trí ban đầu
        GameObject fire = Instantiate(firePrefab, transform.position, Quaternion.identity);

        float elapsedTime = 0f; // Thời gian đã trôi qua

        while (Vector3.Distance(fire.transform.position, target) > 0.1f &&
               Vector3.Distance(initialPosition, fire.transform.position) < maxDistance &&
               elapsedTime < lifetime)
        {
            // Di chuyển viên đạn về phía mục tiêu
            fire.transform.position = Vector3.MoveTowards(fire.transform.position, target, speed * Time.deltaTime);
            elapsedTime += Time.deltaTime; // Cập nhật thời gian đã trôi qua
            yield return null;
        }

        // Kiểm tra va chạm với người chơi
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            // Gây sát thương cho người chơi
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount, penetration);
            }
        }

        // Hủy viên đạn sau khi va chạm hoặc đạt khoảng cách tối đa
        Destroy(fire);
    }
}