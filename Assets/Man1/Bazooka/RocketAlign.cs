using UnityEngine;

public class RocketAlign : MonoBehaviour
{
    private Rigidbody _rb; // Biến để tham chiếu tới Rigidbody của tên lửa

    // Phương thức Start() được gọi một lần khi đối tượng được khởi tạo
    private void Start()
    {
        _rb = GetComponent<Rigidbody>(); // Lấy Rigidbody của tên lửa gắn vào đối tượng này
    }

    // Phương thức Update() được gọi mỗi frame
    private void Update()
    {
        // Nếu vận tốc của tên lửa đủ lớn, tính toán hướng của tên lửa
        if (_rb.velocity.sqrMagnitude > 0.1f) // Kiểm tra xem vận tốc của tên lửa có đủ lớn không (so với một giá trị nhỏ)
        {
            // Cập nhật rotation của tên lửa sao cho mũi tên lửa (với vector forward) hướng theo vận tốc
            transform.rotation = Quaternion.LookRotation(_rb.velocity); // Cập nhật rotation của tên lửa
        }
    }
}
