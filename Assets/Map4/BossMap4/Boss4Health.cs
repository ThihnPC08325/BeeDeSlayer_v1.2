using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    [SerializeField] private GameObject gate; // Cánh cổng

    private AudioSource audioSource;
    private AudioSource backgroundAudioSource;
    private BoxCollider boxCollider;
    private bool isReviving = false;
    private bool isPhase2 = false;
    private Vector3 originalPosition;
    private Quaternion originalRotation;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        audioSource = gameObject.AddComponent<AudioSource>();
        backgroundAudioSource = gameObject.AddComponent<AudioSource>();

        backgroundAudioSource.loop = true;
        backgroundAudioSource.volume = 0.5f;

        if (bossRenderer == null)
            bossRenderer = GetComponentInChildren<Renderer>();

        if (bossLight != null)
            bossLight.enabled = false;

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
            if (bossHealthBar != null) bossHealthBar.gameObject.SetActive(false);
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
        if (backgroundAudioSource.isPlaying)
            backgroundAudioSource.Stop();

        if (deathSound != null)
            audioSource.PlayOneShot(deathSound);

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
    }

    private IEnumerator DeathSequence()
    {
        // Boss chết sẽ chuyển qua phase 2 (triệu hồi cổng)
        isPhase2 = true;
        
        // Triệu hồi cổng sau khi boss chết
        if (gate != null)
        {
            gate.SetActive(true); // Bật cổng khi boss chết
        }

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

        if (bossRenderer != null && phase2Material != null)
            bossRenderer.material = phase2Material;

        yield return StartCoroutine(RotateOverTime(originalRotation, tiltDuration));
        Vector3 riseTargetPosition = sinkTargetPosition + Vector3.up * 500f;
        yield return StartCoroutine(MoveOverTime(riseTargetPosition, riseDuration));

        Revive();
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
            Destroy(gameObject); // Sau khi phase 2, boss sẽ bị hủy
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

        // Cổng sẽ chỉ xuất hiện khi boss chết ở phase 2
        if (gate != null && !gate.activeInHierarchy)
        {
            gate.SetActive(true); // Triệu hồi cổng khi boss chết lần thứ 2
        }

        isReviving = false;
    }
}
