using UnityEngine;

public class ElectricTrap : MonoBehaviour
{
    [SerializeField] float damagePerSecond = 10f;  // Sát thương mỗi giây
    [SerializeField] float slowMultiplier = 0.5f;  // Giảm tốc độ Bee xuống 50%
    [SerializeField] float slowDuration = 2f;      // Bee bị chậm trong 2 giây
    [SerializeField] float activeTime = 3f;        // Bẫy hoạt động trong 3 giây
    [SerializeField] float rechargeTime = 5f;      // Bẫy sạc lại trong 5 giây
    [SerializeField] AudioClip electricTrapSound;  // Âm thanh khi bẫy kích hoạt

    [SerializeField] private bool _isActive = true;
    [SerializeField] private ParticleSystem _electricEffect;
    [SerializeField] private AudioSource _audioSource;

    private void Start()
    {
        _electricEffect = GetComponentInChildren<ParticleSystem>();
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

        if (!other.CompareTag("Player")) return; // Chỉ ảnh hưởng đến Bee
        PlayerHealth player = other.GetComponent<PlayerHealth>();
        PlayerController movement = other.GetComponent<PlayerController>();

        if (player != null)
        {
            player.TakeDamage(damagePerSecond * Time.deltaTime, 0f);
        }

        if (movement != null)
        {
            movement.ModifySpeed(slowMultiplier, slowDuration);
        }
    }

    private void ActivateTrap()
    {
        _isActive = true;
        if (_electricEffect != null) _electricEffect.Play();

        // Phát âm thanh nếu có
        if (_audioSource != null && electricTrapSound != null)
        {
            _audioSource.PlayOneShot(electricTrapSound);
        }

        Invoke(nameof(DeactivateTrap), activeTime);
    }

    private void DeactivateTrap()
    {
        _isActive = false;
        if (_electricEffect != null) _electricEffect.Stop();
        Invoke(nameof(ReactivateTrap), rechargeTime);
    }

    private void ReactivateTrap()
    {
        ActivateTrap();
    }
}
