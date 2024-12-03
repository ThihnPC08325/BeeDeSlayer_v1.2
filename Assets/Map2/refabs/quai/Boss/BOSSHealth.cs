using System.Collections;
using UnityEngine;
using static EnemyManager;
using UnityEngine.UI;

public class BOSSHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    [SerializeField] private float currentHealth;
    [SerializeField] private GameObject smokePrefab;
    [SerializeField] private Slider bossHealthBar;
    [SerializeField] private AudioClip deathSound; // Âm thanh khi chết
    [SerializeField] private AudioClip backgroundMusic; // Nhạc nền khi đang chiến đấu 🎵
    

    private AudioSource audioSource; // Component phát nhạc cho hiệu ứng
    private AudioSource backgroundAudioSource; // Component phát nhạc nền
    private BoxCollider boxCollider;
    private bool isVictoryMusicPlaying = false; // Kiểm tra đã phát nhạc chiến thắng chưa

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();

        // Tạo 2 AudioSource riêng biệt
        audioSource = gameObject.AddComponent<AudioSource>();
        backgroundAudioSource = gameObject.AddComponent<AudioSource>();

        // Cài đặt cho nhạc nền
        backgroundAudioSource.loop = true; // Lặp lại
        backgroundAudioSource.volume = 0.5f; // Âm lượng nhỏ hơn

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
        if (backgroundMusic != null)
        {
            backgroundAudioSource.clip = backgroundMusic;
            backgroundAudioSource.Play();
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0f)
        {
            currentHealth = 0f;
            boxCollider.enabled = false;
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
        if (backgroundAudioSource.isPlaying)
        {
            backgroundAudioSource.Stop();
        }


        // Phát âm thanh chết
        if (deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
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