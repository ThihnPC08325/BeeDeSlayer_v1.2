using UnityEngine;

public class MinimapRotation : MonoBehaviour
{
    public Transform player; // Gán Player vào đây

    void Update()
    {
        if (player == null) return;

        // Chỉ xoay theo trục Y để giữ góc nhìn từ trên xuống
        transform.rotation = Quaternion.Euler(90f, player.eulerAngles.y, 0f);
    }
}
