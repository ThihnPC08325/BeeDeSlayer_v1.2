using UnityEngine;

public class ElectricTrap : MonoBehaviour
{
    public float damagePerSecond = 10f;  // Sát thương mỗi giây
    public float slowMultiplier = 0.5f;  // Giảm tốc độ Bee xuống 50%
    public float slowDuration = 2f;      // Bee bị chậm trong 2 giây
    public float activeTime = 3f;        // Bẫy hoạt động trong 3 giây
    public float rechargeTime = 5f;      // Bẫy sạc lại trong 5 giây

    private bool _isActive = true;
    private ParticleSystem _electricEffect;

    private void Start()
    {
        _electricEffect = GetComponentInChildren<ParticleSystem>();
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
