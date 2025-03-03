using System.Collections;
using UnityEngine;

public class BazookaSkill : MonoBehaviour
{
    private static readonly int FireBazooka = Animator.StringToHash("FireBazooka");

    [Header("Bazooka Settings")]
    [SerializeField] private GameObject rocketPrefab;   // Prefab Rocket đã chỉnh sửa
    [SerializeField] private Transform firePoint;         // FirePoint được đặt ở miệng súng Bazooka
    [SerializeField] private float rocketSpeed = 15f;       // Vận tốc ban đầu của Rocket
    [SerializeField] private float cooldownTime = 5f;       // Thời gian hồi chiêu

    [Header("Animation")]
    [SerializeField] private Animator playerAnimator;       // Animator của cánh tay hoặc model súng Bazooka

    private bool _canFire = true; // Kiểm tra xem có thể bắn hay không


    void Update()
    {
        // Bắn rocket khi nhấn chuột trái
        if (Input.GetMouseButtonDown(0) && _canFire)
        {
            FireRocket();
        }
    }


    private void FireRocket()
    {
        if (!_canFire) return; // Nếu không thể bắn, không làm gì cả
        _canFire = false; // Đặt _canFire thành false để ngừng việc bắn cho đến khi cooldown kết thúc

        // Kích hoạt animation bằng cách gọi trigger "FireBazooka"
        if (playerAnimator)
        {
            playerAnimator.SetTrigger(FireBazooka); // Gọi trigger "FireBazooka" trong Animator để thực hiện hoạt ảnh bắn
        }

        // Instantiate Rocket tại vị trí firePoint với rotation của firePoint
        GameObject rocket = Instantiate(rocketPrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = rocket.GetComponent<Rigidbody>();
        if (rb)
        {
            rb.AddForce(firePoint.forward * rocketSpeed, ForceMode.Impulse); // Thêm lực để tên lửa bay theo hướng của firePoint
        }

        // Khởi động Coroutine để quản lý cooldown
        StartCoroutine(RocketCooldown());
    }


    private IEnumerator ResetBazookaTrigger(float delay)
    {
        yield return new WaitForSeconds(delay); // Đợi một khoảng thời gian delay
        if (playerAnimator != null)
        {
            playerAnimator.ResetTrigger("FireBazooka"); // Reset trigger "FireBazooka" trong Animator
        }
    }


    private IEnumerator RocketCooldown()
    {
        yield return new WaitForSeconds(cooldownTime); // Đợi trong khoảng thời gian cooldownTime (ví dụ 5 giây)
        _canFire = true; // Sau khi cooldown kết thúc, cho phép bắn tên lửa tiếp
    }

}
