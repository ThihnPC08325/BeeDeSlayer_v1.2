using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill2Boss : MonoBehaviour
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private float damagePen = 0f;

    [SerializeField] private float rotationSpeedX = 10f;
    [SerializeField] private float rotationSpeedY = 20f;
    [SerializeField] private float rotationSpeedZ = 30f;

    // Thêm danh sách hiệu ứng âm thanh
    [SerializeField] private AudioClip[] destroySounds;

    // Prefab Particle System
    [SerializeField] private GameObject particlePrefab;

    private AudioSource audioSource;

    private void Start()
    {
        // Gắn hoặc tạo AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;
        transform.Rotate(rotationSpeedX * deltaTime, rotationSpeedY * deltaTime, rotationSpeedZ * deltaTime);

        rotationSpeedX += 0.1f * deltaTime;
        rotationSpeedY += 0.1f * deltaTime;
        rotationSpeedZ += 0.1f * deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage, damagePen);
            }
            CameraShake.Instance.TriggerShake(0.8f, 2f);
            // Gọi hiệu ứng particle system
            InstantiateParticleEffect();
            // Gọi hiệu ứng âm thanh
            PlayDestroySound();
        }
        if (other.CompareTag("Ground"))
        {
            CameraShake.Instance.TriggerShake(0.5f, 0.5f);
            // Gọi hiệu ứng particle system
            InstantiateParticleEffect();
            // Gọi hiệu ứng âm thanh
            PlayDestroySound();
        }
    }

    private void PlayDestroySound()
    {
        if (destroySounds != null && destroySounds.Length > 0)
        {
            // Chọn ngẫu nhiên một âm thanh từ mảng
            AudioClip randomSound = destroySounds[Random.Range(0, destroySounds.Length)];
            audioSource.PlayOneShot(randomSound);

            // Trì hoãn hủy đối tượng theo độ dài của âm thanh
            float destroyDelay = randomSound.length;
            Destroy(gameObject, destroyDelay);
        }
        else
        {
            // Nếu không có âm thanh, hủy ngay lập tức
            Destroy(gameObject);
        }
    }

    private void InstantiateParticleEffect()
    {
        if (particlePrefab != null)
        {
            // Instantiate particle system tại vị trí của đối tượng
            Instantiate(particlePrefab, transform.position, Quaternion.identity);
        }
    }
}
