using UnityEngine;

public class ElectricTrap : MonoBehaviour
{
    public float damagePerSecond = 10f;  // Sát thương mỗi giây
    public float slowMultiplier = 0.5f;  // Giảm tốc độ Bee xuống 50%
    public float slowDuration = 2f;      // Bee bị chậm trong 2 giây
    public float activeTime = 3f;        // Bẫy hoạt động trong 3 giây
    public float rechargeTime = 5f;      // Bẫy sạc lại trong 5 giây

    private bool isActive = true;
    private ParticleSystem electricEffect;

    private void Start()
    {
        electricEffect = GetComponentInChildren<ParticleSystem>();
        ActivateTrap();
    }

    private void OnTriggerStay(Collider other)
    {
        if (!isActive) return;

        if (other.CompareTag("Player")) // Chỉ ảnh hưởng đến Bee
        {
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
    }

    private void ActivateTrap()
    {
        isActive = true;
        if (electricEffect != null) electricEffect.Play();
        Invoke("DeactivateTrap", activeTime);
    }

    private void DeactivateTrap()
    {
        isActive = false;
        if (electricEffect != null) electricEffect.Stop();
        Invoke("ReactivateTrap", rechargeTime);
    }

    private void ReactivateTrap()
    {
        ActivateTrap();
    }
}
