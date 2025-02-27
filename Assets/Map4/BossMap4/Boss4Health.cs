using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Boss4Health : MonoBehaviour
{
    [SerializeField] private float maxHealth = 1000f;
    [SerializeField] private float currentHealth;
    [SerializeField] private GameObject smokePrefab;
    [SerializeField] private Slider bossHealthBar;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private Material skyboxPhase2;
    [SerializeField] private Material phase2Material;
    [SerializeField] private Renderer bossRenderer;
    [SerializeField] private Light bossLight;

<<<<<<< Updated upstream

    private AudioSource audioSource; // Component phát nhạc cho hiệu ứng
    private AudioSource backgroundAudioSource; // Component phát nhạc nền
    private BoxCollider boxCollider;
    private bool isVictoryMusicPlaying = false; // Kiểm tra đã phát nhạc chiến thắng chưa
=======
    private AudioSource audioSource;
    private AudioSource backgroundAudioSource;
    private BoxCollider boxCollider;
    private bool isReviving = false;
    private bool isPhase2 = false;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
>>>>>>> Stashed changes

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
<<<<<<< Updated upstream

        // Tạo 2 AudioSource riêng biệt
        audioSource = gameObject.AddComponent<AudioSource>();
        backgroundAudioSource = gameObject.AddComponent<AudioSource>();

        // Cài đặt cho nhạc nền
        backgroundAudioSource.loop = true; // Lặp lại
        backgroundAudioSource.volume = 0.5f; // Âm lượng nhỏ hơn
=======
        audioSource = gameObject.AddComponent<AudioSource>();
        backgroundAudioSource = gameObject.AddComponent<AudioSource>();

        backgroundAudioSource.loop = true;
        backgroundAudioSource.volume = 0.5f;

        if (bossRenderer == null)
            bossRenderer = GetComponentInChildren<Renderer>();

        if (bossLight != null)
            bossLight.enabled = false;
>>>>>>> Stashed changes

        currentHealth = maxHealth;
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    void Start()
    {
        currentHealth = maxHealth;
        if (bossHealthBar != null)
        {
            bossHealthBar.maxValue = maxHealth;
            bossHealthBar.value = currentHealth;
        }

<<<<<<< Updated upstream
        // Phát nhạc nền khi bắt đầu
=======
>>>>>>> Stashed changes
        if (backgroundMusic != null)
        {
            backgroundAudioSource.clip = backgroundMusic;
            backgroundAudioSource.Play();
        }
    }

    public void TakeDamage(float damage)
    {
        if (isReviving) return;

        currentHealth -= damage;
        if (currentHealth <= 0f)
        {
            currentHealth = 0f;
            boxCollider.enabled = false;
<<<<<<< Updated upstream
=======
            if (bossHealthBar != null) bossHealthBar.gameObject.SetActive(false);
>>>>>>> Stashed changes
            Die();
        }
        else
        {
            if (bossHealthBar != null)
                bossHealthBar.value = currentHealth;
        }
    }

    private void Die()
    {
<<<<<<< Updated upstream
        // Dừng nhạc nền
        if (backgroundAudioSource.isPlaying)
        {
            backgroundAudioSource.Stop();
        }
=======
        if (backgroundAudioSource.isPlaying)
            backgroundAudioSource.Stop();
>>>>>>> Stashed changes

        if (deathSound != null)
<<<<<<< Updated upstream
        {
            audioSource.PlayOneShot(deathSound);
        }
=======
            audioSource.PlayOneShot(deathSound);
>>>>>>> Stashed changes

        if (smokePrefab != null)
            Instantiate(smokePrefab, transform.position, Quaternion.identity);

        if (skyboxPhase2 != null && !isPhase2)
        {
            RenderSettings.skybox = skyboxPhase2;
            DynamicGI.UpdateEnvironment();
        }

        if (bossLight != null)
            StartCoroutine(IncreaseLightIntensity());

        StartCoroutine(DeathSequence());
    }

    private IEnumerator IncreaseLightIntensity()
    {
<<<<<<< Updated upstream
        yield return new WaitForSeconds(duration);
        gameObject.SetActive(false);
        //Test
=======
        bossLight.enabled = true;
        float duration = 5f;
        float elapsed = 0f;
        bossLight.intensity = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            bossLight.intensity = Mathf.Lerp(0f, 100000f, elapsed / duration);
            yield return null;
        }
>>>>>>> Stashed changes
    }

    private IEnumerator DeathSequence()
    {
        float tiltDuration = 5f;
        float sinkDuration = 5f;
        float changeAppearanceDelay = 4f;
        float riseDuration = 5f;

        Quaternion tiltedRotation = Quaternion.Euler(90f, originalRotation.eulerAngles.y, originalRotation.eulerAngles.z);
        yield return StartCoroutine(RotateOverTime(tiltedRotation, tiltDuration));
        yield return new WaitForSeconds(0.5f);

        Vector3 sinkTargetPosition = originalPosition + Vector3.down * 500f;
        yield return StartCoroutine(MoveOverTime(sinkTargetPosition, sinkDuration));
        yield return new WaitForSeconds(changeAppearanceDelay);

        if (!isPhase2)
        {
            if (bossRenderer != null && phase2Material != null)
                bossRenderer.material = phase2Material;

            yield return StartCoroutine(RotateOverTime(originalRotation, tiltDuration));
            Vector3 riseTargetPosition = sinkTargetPosition + Vector3.up * 500f;
            yield return StartCoroutine(MoveOverTime(riseTargetPosition, riseDuration));
            Revive();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator RotateOverTime(Quaternion targetRotation, float duration)
    {
        float elapsed = 0f;
        Quaternion startRotation = transform.rotation;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, elapsed / duration);
            yield return null;
        }
    }

    private IEnumerator MoveOverTime(Vector3 targetPosition, float duration)
    {
        float elapsed = 0f;
        Vector3 startPosition = transform.position;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsed / duration);
            yield return null;
        }
    }

    private void Revive()
    {
        if (isPhase2)
        {
            Destroy(gameObject);
            return;
        }

        isPhase2 = true;
        maxHealth *= 10;
        currentHealth = maxHealth;

        if (bossHealthBar != null)
        {
            bossHealthBar.maxValue = maxHealth;
            bossHealthBar.value = currentHealth;
            bossHealthBar.gameObject.SetActive(true);
        }

        boxCollider.enabled = true;
        isReviving = false;
    }
}
