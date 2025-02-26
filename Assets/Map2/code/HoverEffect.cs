using UnityEngine;

public class HoverEffect : MonoBehaviour
{
    public float hoverSpeed = 2.0f; // Tốc độ nhún
    public float hoverHeight = 0.5f; // Biên độ nhún
    private Vector3 _startPosition;

    private void Start()
    {
        // Lưu vị trí ban đầu
        _startPosition = transform.position;
    }

    private void Update()
    {
        // Tạo chuyển động lên xuống
        float newY = _startPosition.y + Mathf.Sin(Time.time * hoverSpeed) * hoverHeight;
        transform.position = new Vector3(_startPosition.x, newY, _startPosition.z);
    }
}

