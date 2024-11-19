using UnityEngine;

public class HoverEffect : MonoBehaviour
{
    public float hoverSpeed = 2.0f; // Tốc độ nhún
    public float hoverHeight = 0.5f; // Biên độ nhún
    private Vector3 startPosition;

    void Start()
    {
        // Lưu vị trí ban đầu
        startPosition = transform.position;
    }

    void Update()
    {
        // Tạo chuyển động lên xuống
        float newY = startPosition.y + Mathf.Sin(Time.time * hoverSpeed) * hoverHeight;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }
}

