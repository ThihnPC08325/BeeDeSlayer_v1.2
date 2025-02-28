using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // Thêm thư viện để chuyển Scene

public class BOSSHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    [SerializeField] private float currentHealth;
    [SerializeField] private GameObject smokePrefab;
    [SerializeField] private Slider bossHealthBar;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private GameObject exitZone; // Vùng chuyển scene

    private AudioSource audioSource;
    private AudioSource backgroundAudioSource;
    private BoxCollider boxCollider;
    private bool isVictoryMusicPlaying = false;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        audioSource = gameObject.AddComponent<AudioSource>();
        backgroundAudioSource = gameObject.AddComponent<AudioSource>();

        backgroundAudioSource.loop = true;
        backgroundAudioSource.volume = 0.5f;

        currentHealth = maxHealth;

        if (exitZone != null)
        {
            exitZone.SetActive(false); // Ẩn vùng chuyển scene ban đầu
        }
    }

    void Start()
    {
        currentHealth = maxHealth;

        if (bossHealthBar != null)
        {
            bossHealthBar.maxValue = maxHealth;
            bossHealthBar.value = currentHealth;
        }

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
        if (backgroundAudioSource.isPlaying)
        {
            backgroundAudioSource.Stop();
        }

        if (deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

        if (smokePrefab != null)
        {
            Instantiate(smokePrefab, transform.position, Quaternion.identity);
        }

        StartCoroutine(TimeToDie(2f));
    }

    private IEnumerator TimeToDie(float duration)
    {
        yield return new WaitForSeconds(duration);
        gameObject.SetActive(false); // Ẩn Boss

        if (exitZone != null)
        {
            exitZone.SetActive(true); // Hiện vùng chuyển scene
        }
    }
}
