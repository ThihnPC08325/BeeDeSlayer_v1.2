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
    [SerializeField] private float zMin = -50f; // Giới hạn nhỏ nhất của trục Z
    [SerializeField] private float zMax = 50f;  // Giới hạn lớn nhất của trục Z



    private void Start()
    {
        _fireEffect = GetComponentInChildren<ParticleSystem>();
        _audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();

        ActivateTrap();
    }

    private void OnTriggerStay(Collider other)
    {
        if (!_isActive || !other.CompareTag("Player")) return;

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

        RelocateTrap(); // Đặt lại vị trí trước khi kích hoạt lại
        Invoke(nameof(ReactivateTrap), rechargeTime);
    }

    private void ReactivateTrap()
    {
        ActivateTrap();
    }

    private void RelocateTrap()
    {
        // Lấy vị trí hiện tại
        Vector3 newPosition = transform.position;

        // Random giá trị mới cho trục Z trong khoảng giới hạn
        newPosition.z = Random.Range(zMin, zMax);

        // Cập nhật vị trí
        transform.position = newPosition;

        Debug.Log($"🔥 FireTrap di chuyển đến Z: {newPosition.z}");
    }

    
}