using System.Collections;
using UnityEngine;
using static EnemyManager;
using UnityEngine.UI;

public class Boss4Health : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    [SerializeField] private float currentHealth;
    [SerializeField] private GameObject smokePrefab;
    [SerializeField] private Slider bossHealthBar;
    [SerializeField] private AudioClip deathSound; // Âm thanh khi chết
    [SerializeField] private AudioClip backgroundMusic; // Nhạc nền khi đang chiến đấu 🎵


    private AudioSource _audioSource; // Component phát nhạc cho hiệu ứng
    private AudioSource _backgroundAudioSource; // Component phát nhạc nền
    private BoxCollider _boxCollider;

    private void Awake()
    {
        _boxCollider = GetComponent<BoxCollider>();

        // Tạo 2 AudioSource riêng biệt
        _audioSource = gameObject.AddComponent<AudioSource>();
        _backgroundAudioSource = gameObject.AddComponent<AudioSource>();

        // Cài đặt cho nhạc nền
        _backgroundAudioSource.loop = true; // Lặp lại
        _backgroundAudioSource.volume = 0.5f; // Âm lượng nhỏ hơn

        currentHealth = maxHealth;
    }

    void Start()
    {
        currentHealth = maxHealth;

        if (bossHealthBar != null)
        {
            bossHealthBar.maxValue = maxHealth;
            bossHealthBar.value = currentHealth;
        }

        // Phát nhạc nền khi bắt đầu
        if (backgroundMusic == null) return;
        _backgroundAudioSource.clip = backgroundMusic;
        _backgroundAudioSource.Play();
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0f)
        {
            currentHealth = 0f;
            _boxCollider.enabled = false;
            Die();
        }
        if (bossHealthBar != null)
        {
            bossHealthBar.value = currentHealth;
        }
    }

    private void Die()
    {
        // Dừng nhạc nền
        if (_backgroundAudioSource.isPlaying)
        {
            _backgroundAudioSource.Stop();
        }


        // Phát âm thanh chết
        if (deathSound != null)
        {
            _audioSource.PlayOneShot(deathSound);
        }

        // Hiệu ứng khói
        if (smokePrefab != null)
        {
            Instantiate(smokePrefab, transform.position, Quaternion.identity);
        }

        StartCoroutine(TimeToDie(2f));
    }

    private IEnumerator TimeToDie(float duration)
    {
        yield return new WaitForSeconds(duration);
        gameObject.SetActive(false);
    }
}