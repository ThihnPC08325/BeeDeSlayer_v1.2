using System.Collections;
using UnityEngine;

public class BazookaSkill : MonoBehaviour
{
    [Header("Bazooka Settings")]
    [SerializeField] private GameObject rocketPrefab;   // Prefab Rocket đã chỉnh sửa
    [SerializeField] private Transform firePoint;         // FirePoint được đặt ở miệng súng Bazooka
    [SerializeField] private float rocketSpeed = 15f;       // Vận tốc ban đầu của Rocket
    [SerializeField] private float cooldownTime = 5f;       // Thời gian hồi chiêu

    // Nếu không cần hiệu ứng xoay, có thể xóa dòng torque này
    //[SerializeField] private float torqueAmount = 0.5f;

    private bool canFire = true;

    void Update()
    {
        // Sử dụng chuột trái để bắn Rocket
        if (Input.GetMouseButtonDown(0) && canFire)
        {
            FireRocket();
        }
    }

    private void FireRocket()
    {
        if (!canFire) return;
        canFire = false;

        // Instantiate Rocket với rotation của firePoint
        GameObject rocket = Instantiate(rocketPrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = rocket.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Không freeze rotation để Rocket có thể xoay theo vận tốc
            // rb.constraints = RigidbodyConstraints.FreezeRotation; // Bỏ dòng này

            // Áp dụng lực xung ban đầu với AddForce
            rb.AddForce(firePoint.forward * rocketSpeed, ForceMode.Impulse);
        }

        StartCoroutine(RocketCooldown());
    }


    private IEnumerator RocketCooldown()
    {
        yield return new WaitForSeconds(cooldownTime);
        canFire = true;
    }
}
