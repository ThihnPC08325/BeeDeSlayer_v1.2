using UnityEngine;
using System.Collections;

public class Trap : MonoBehaviour
{
    public float damagePerSecond = 5f;  // Sát thương mỗi giây
    public float activeTime = 3f;       // Bẫy hoạt động trong 3 giây
    public float rechargeTime = 5f;     // Thời gian sạc lại bẫy

    private bool _isActive = true;      // Kiểm tra bẫy có đang hoạt động không
    private Coroutine damageCoroutine;  // Biến lưu Coroutine trừ máu

    private void Start()
    {
        ActivateTrap();
    }

    private void OnTriggerStay(Collider other)
    {
        if (!_isActive) return; // Nếu bẫy đang tắt, không gây sát thương

        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damagePerSecond * Time.deltaTime, 0f);
            }
        }
    }

    private void ActivateTrap()
    {
        _isActive = true;
        Invoke(nameof(DeactivateTrap), activeTime);
    }

    private void DeactivateTrap()
    {
        _isActive = false;
        Invoke(nameof(ReactivateTrap), rechargeTime);
    }

    private void ReactivateTrap()
    {
        ActivateTrap();
    }
}
