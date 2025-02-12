using System.Collections;
using UnityEngine;

public class BazookaSkill : MonoBehaviour
{
    [Header("Bazooka Settings")]
    [SerializeField] private GameObject rocketPrefab;   // Prefab Rocket đã chỉnh sửa
    [SerializeField] private Transform firePoint;         // FirePoint được đặt ở miệng súng Bazooka
    [SerializeField] private float rocketSpeed = 15f;       // Vận tốc ban đầu của Rocket
    [SerializeField] private float cooldownTime = 5f;       // Thời gian hồi chiêu

    [Header("Animation")]
    [SerializeField] private Animator playerAnimator;       // Animator của cánh tay hoặc model súng Bazooka

    private bool canFire = true;

    void Update()
    {
        // Bắn rocket khi nhấn chuột trái
        if (Input.GetMouseButtonDown(0) && canFire)
        {
            FireRocket();
        }
    }

    private void FireRocket()
    {
        if (!canFire) return;
        canFire = false;
        Debug.Log(canFire);
        // Kích hoạt animation bằng cách gọi trigger "FireBazooka"
        if (playerAnimator != null)
        {
            Debug.Log("animtion start");
            playerAnimator.SetTrigger("FireBazooka");
            // Nếu cần, bạn có thể reset trigger sau một khoảng thời gian bằng coroutine
            /*StartCoroutine(ResetBazookaTrigger(0.5f));*/ // 0.5f là thời gian chờ, điều chỉnh theo length của animation
        }

        // Instantiate Rocket tại vị trí firePoint với rotation của firePoint
        GameObject rocket = Instantiate(rocketPrefab, firePoint.position, firePoint.rotation);
        Debug.Log(rocket);
        Rigidbody rb = rocket.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Debug.Log(canFire);
            rb.AddForce(firePoint.forward * rocketSpeed, ForceMode.Impulse);
        }

        StartCoroutine(RocketCooldown());
    }

    // Coroutine để reset trigger sau khi animation hoàn thành (nếu cần)
    private IEnumerator ResetBazookaTrigger(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (playerAnimator != null)
        {
            playerAnimator.ResetTrigger("FireBazooka");
        }
    }

    private IEnumerator RocketCooldown()
    {
        yield return new WaitForSeconds(cooldownTime);
        canFire = true;
    }
}
