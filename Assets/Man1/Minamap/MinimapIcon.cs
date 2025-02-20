using UnityEngine;
using UnityEngine.UI;

public class MinimapIcon : MonoBehaviour
{
    public Transform player; // Player thật
    public RectTransform minimapIcon; // Icon trên minimap
    public RectTransform minimapUI; // Khu vực minimap UI
    public float mapSize = 50f; // Kích thước map thật

    void Update()
    {
        if (player == null) return;

        // Chuyển vị trí Player từ thế giới sang minimap
        Vector2 playerPos = new Vector2(player.position.x / mapSize, player.position.z / mapSize);
        playerPos *= minimapUI.sizeDelta; // Chuyển sang tọa độ UI

        // Cập nhật vị trí icon trong minimap UI
        minimapIcon.anchoredPosition = playerPos;
    }
}
