using System.Collections;
using UnityEngine;

public class LaserScript : MonoBehaviour
{
    [Header("Laser Settings")]
    [SerializeField] private float laserDamage = 2f; // Sát thương của laser
    [SerializeField] private float laserDuration = 0.5f; // Thời gian tồn tại của laser
    [SerializeField] private float laserWidth = 0.1f; // Độ rộng của laser
    private LineRenderer lineRenderer; // Để vẽ laser
    private Vector3 targetPosition; // Vị trí mục tiêu

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = laserWidth;
        lineRenderer.endWidth = laserWidth;
    }

    public void SetTarget(Vector3 target)
    {
        targetPosition = target;
    }

    public void StartLaser(float duration)
    {
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, targetPosition);

        // Bắt đầu Coroutine để gây sát thương
        StartCoroutine(DamagePlayer(duration));

        // Kết thúc laser sau một khoảng thời gian
        StartCoroutine(EndLaser(duration));
    }

    private IEnumerator DamagePlayer(float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // Kiểm tra va chạm với người chơi
            Collider[] hitColliders = Physics.OverlapCapsule(transform.position, targetPosition, laserWidth / 2);
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Player"))
                {
                    PlayerHealth playerHealth = hitCollider.GetComponent<PlayerHealth>();
                    if (playerHealth != null)
                    {
                        playerHealth.TakeDamage(laserDamage, 0); // Gọi phương thức TakeDamage
                    }
                }
            }

            elapsedTime += Time.deltaTime;
            yield return null; // Đợi đến frame tiếp theo
        }
    }

    private IEnumerator EndLaser(float duration)
    {
        yield return new WaitForSeconds(duration);
        // Tắt laser
        lineRenderer.enabled = false;
        Destroy(gameObject); // Huỷ đối tượng laser
    }
}