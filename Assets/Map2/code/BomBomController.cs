using System.Collections;
using UnityEngine;

public class BomBomcontroller : MonoBehaviour
{
    [Header("BomBom Settings")]
    [SerializeField] private float moveSpeed = 3f; // Tốc độ di chuyển
    [SerializeField] private float detectionRange = 10f; // Phạm vi phát hiện người chơi
    [SerializeField] private float explosionRange = 3f; // Phạm vi phát nổ
    [SerializeField] private float explosionDamage = 100f; // Sát thương vụ nổ
    [SerializeField] private float explosionDelay = 1.5f; // Độ trễ trước khi phát nổ
    [SerializeField] private GameObject explosionEffect; // Hiệu ứng nổ
    [SerializeField] private AudioClip explosionSound;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Renderer bomRenderer; // Renderer của BomBom
    [SerializeField] private Color flashColor = Color.red; // Màu nhấp nháy
    [SerializeField] private float flashFrequency = 0.2f; // Tần số nhấp nháy (giây)

    private Transform _player;
    private bool _isExploding = false;
    private Color _originalColor; // Màu ban đầu của BomBom
    private Coroutine _flashCoroutine;

    private void Start()
    {
        // Tìm đối tượng người chơi qua tag "Player"
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            _player = playerObject.transform;
        }

        // Lưu màu ban đầu của BomBom
        if (bomRenderer != null)
        {
            _originalColor = bomRenderer.material.color;
        }
    }

    private void Update()
    {
        if (!_player || _isExploding) return;

        // Tính khoảng cách đến người chơi
        float distanceToPlayer = Vector3.Distance(transform.position, _player.position);

        if (!(distanceToPlayer <= detectionRange)) return;
        MoveTowardsPlayer();
        if (distanceToPlayer <= explosionRange)
        {
            StartExplosion();
        }
    }

    private void MoveTowardsPlayer()
    {
        // Di chuyển về phía người chơi
        Vector3 direction = (_player.position - transform.position).normalized;
        transform.position += direction * (moveSpeed * Time.deltaTime);

        // Quay mặt về phía người chơi
        transform.LookAt(_player);
    }

    private void StartExplosion()
    {
        if (!_isExploding)
        {
            _isExploding = true;
            Debug.Log("BomBom bắt đầu phát nổ!");

            // Bắt đầu hiệu ứng nhấp nháy
            if (bomRenderer)
            {
                _flashCoroutine = StartCoroutine(FlashEffect());
            }

            Invoke(nameof(Explode), explosionDelay);
        }
    }

    private void Explode()
    {
        // Dừng nhấp nháy và khôi phục màu ban đầu
        if (_flashCoroutine != null)
        {
            StopCoroutine(_flashCoroutine);
            if (bomRenderer != null)
            {
                bomRenderer.material.color = _originalColor;
            }
        }

        // Hiệu ứng nổ
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        // Âm thanh nổ
        if (audioSource != null && explosionSound != null)
        {
            audioSource.PlayOneShot(explosionSound);
        }

        // Gây sát thương
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRange);
        foreach (Collider hit in hitColliders)
        {
            if (!hit.CompareTag("Player")) continue;
            PlayerHealth playerHealth = hit.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(explosionDamage, 0f);
            }
        }

        // Phá hủy BomBom
        Destroy(gameObject);
        Debug.Log("BomBom phát nổ!");
    }

    private IEnumerator FlashEffect()
    {
        while (true)
        {
            // Chuyển sang màu nhấp nháy
            bomRenderer.material.color = flashColor;
            yield return new WaitForSeconds(flashFrequency);

            // Khôi phục màu ban đầu
            bomRenderer.material.color = _originalColor;
            yield return new WaitForSeconds(flashFrequency);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Hiển thị phạm vi phát hiện và phạm vi nổ
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }
}
