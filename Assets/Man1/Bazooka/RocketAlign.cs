using UnityEngine;

public class RocketAlign : MonoBehaviour
{
    private Rigidbody _rb;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // Nếu vận tốc đủ lớn để tính toán hướng, cập nhật rotation của rocket
        if (_rb.velocity.sqrMagnitude > 0.1f)
        {
            // Đặt rotation của rocket sao cho mũi rocket (vector forward) luôn hướng theo vector vận tốc
            transform.rotation = Quaternion.LookRotation(_rb.velocity);
        }
    }
}
