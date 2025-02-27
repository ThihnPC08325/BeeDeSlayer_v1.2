using UnityEngine;

public class FireTrap : MonoBehaviour
{
    [SerializeField] private float damagePerSecond = 10f;
    [SerializeField] private float activeTime = 3f;
    [SerializeField] private float rechargeTime = 5f;
    [SerializeField] private AudioClip fireTrapSound;

    [SerializeField] private bool _isActive = true;
    [SerializeField] private ParticleSystem _fireEffect;
    [SerializeField] private AudioSource _audioSource;

    [SerializeField] private Transform[] spawnPoints; // Danh sách vị trí bẫy có thể xuất hiện

    private void Start()
    {
        _fireEffect = GetComponentInChildren<ParticleSystem>();
        _audioSource = GetComponent<AudioSource>();

        if (_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }

        ActivateTrap();
    }

    private void OnTriggerStay(Collider other)
    {
        if (!_isActive) return;
        if (!other.CompareTag("Player")) return;

        PlayerHealth player = other.GetComponent<PlayerHealth>();

        if (player != null)
        {
            player.TakeDamage(damagePerSecond * Time.deltaTime, 0f);
        }
    }

    private void ActivateTrap()
    {
        _isActive = true;
        if (_fireEffect != null) _fireEffect.Play();

        if (_audioSource != null && fireTrapSound != null)
        {
            _audioSource.PlayOneShot(fireTrapSound);
        }

        Invoke(nameof(DeactivateTrap), activeTime);
    }

    private void DeactivateTrap()
    {
        _isActive = false;
        if (_fireEffect != null) _fireEffect.Stop();

        // Đặt bẫy đến vị trí mới trước khi kích hoạt lại
        RelocateTrap();

        Invoke(nameof(ReactivateTrap), rechargeTime);
    }

    private void ReactivateTrap()
    {
        ActivateTrap();
    }

    private void RelocateTrap()
    {
        if (spawnPoints.Length == 0) return;

        int randomIndex = Random.Range(0, spawnPoints.Length);
        transform.position = spawnPoints[randomIndex].position;
    }
}