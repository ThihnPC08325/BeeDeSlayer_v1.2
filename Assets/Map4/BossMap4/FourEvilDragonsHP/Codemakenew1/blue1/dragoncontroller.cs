using UnityEngine;

public class dragoncontroller : MonoBehaviour
{
    public Transform player; // Tham chiếu đến người chơi
    public float detectionDistance = 10f; // Khoảng cách phát hiện người chơi
    public float rotationSpeed = 5f; // Tốc độ xoay của enemy
    public float moveSpeed = 50f; // Tốc độ di chuyển của enemy

    private void Update()
    {
        // Kiểm tra khoảng cách giữa enemy và người chơi
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance < detectionDistance)
        {
            RotateTowardsPlayer();
            MoveTowardsPlayer();
        }
    }

    private void RotateTowardsPlayer()
    {
        // Tính toán hướng từ enemy đến người chơi
        Vector3 direction = (player.position - transform.position).normalized;

        // Quay mặt enemy về phía người chơi
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    private void MoveTowardsPlayer()
    {
        // Tính toán hướng từ enemy đến người chơi
        Vector3 direction = (player.position - transform.position).normalized;

        // Di chuyển enemy về phía người chơi
        transform.position += direction * moveSpeed * Time.deltaTime;
    }
}