using UnityEngine;

public class MinimapFollow : MonoBehaviour
{
    public Transform player; // Gán Player vào đây
    public float height = 20f; // Độ cao của MinimapCamera

    void LateUpdate()
    {
        if (player == null) return;

        // Cập nhật vị trí camera minimap theo player
        Vector3 newPos = player.position + Vector3.up * height;
        transform.position = newPos;
    }
}
