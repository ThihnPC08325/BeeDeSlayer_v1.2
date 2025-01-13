using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    [SerializeField] private int damage = 5;  // Sát thương gây ra khi va chạm với người chơi
    [SerializeField] private float speed = 100f;  // Tốc độ di chuyển của viên đạn
    [SerializeField] private float lifetime = 15f;  // Thời gian sống của viên đạn
    [SerializeField] private GameObject explosionEffectPrefab;  // Particle Effect khi viên đạn phát nổ
    [SerializeField] private LayerMask playerLayer;  // Layer của Player để nhận diện khi va chạm

    private Rigidbody rb;  // Rigidbody của viên đạn
    private Transform target;  // Vị trí người chơi

    void Start()
    {
        // Lấy vị trí của người chơi
        target = GameObject.FindWithTag("Player")?.transform;

        // Lấy Rigidbody của viên đạn
        rb = GetComponent<Rigidbody>();

        // Tắt trọng lực để viên đạn không rơi xuống đất
        rb.useGravity = false;

        // Nếu tìm thấy người chơi, tính toán hướng bay của viên đạn
        if (target != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;

            // Di chuyển viên đạn theo hướng đến người chơi
            rb.velocity = direction * speed;
        }

        // Tự hủy viên đạn sau thời gian sống
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Kiểm tra nếu viên đạn va chạm với Player
        if (((1 << other.gameObject.layer) & playerLayer) != 0)
        {
            // Gây sát thương cho người chơi khi viên đạn va chạm
            other.GetComponent<PlayerHealth>()?.TakeDamage(damage, 0); // Truyền đúng tham số cho TakeDamage
        }

        // Kiểm tra nếu viên đạn va chạm với Boss
        if (other.CompareTag("Enemy"))
        {
            return;  // Không phát nổ khi va chạm với Boss
        }

        // Phát nổ và hủy viên đạn sau khi va chạm
        Explode();
    }

    // Tạo hiệu ứng nổ
    void Explode()
    {
        // Tạo hiệu ứng nổ tại vị trí viên đạn
        Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);

        // Hủy viên đạn sau khi phát nổ
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        // Hiển thị bán kính vụ nổ trong Editor (nếu có)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, damage); // Dùng sát thương để xác định bán kính vụ nổ
    }

    // Phương thức công khai để truy cập giá trị sát thương
    public void SetDamage(int newDamage)
    {
        damage = newDamage;
    }
}
