using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    private AudioSource _audioSource;
    private AudioSource _backgroundAudioSource;
    private BoxCollider _boxCollider;
    private bool _isReviving = false;
    private bool _isPhase2 = false;
    private Vector3 _originalPosition;
    private Quaternion _originalRotation;

    private void Awake()
    {
        _boxCollider = GetComponent<BoxCollider>();
        _audioSource = gameObject.AddComponent<AudioSource>();
        _backgroundAudioSource = gameObject.AddComponent<AudioSource>();

        _backgroundAudioSource.loop = true;
        _backgroundAudioSource.volume = 0.5f;

        if (bossRenderer == null)
            bossRenderer = GetComponentInChildren<Renderer>();

        if (bossLight != null)
            bossLight.enabled = false;

        currentHealth = maxHealth;
        _originalPosition = transform.position;
        _originalRotation = transform.rotation;
    }

    private void Start()
    {
        currentHealth = maxHealth;
        if (bossHealthBar)
        {
            bossHealthBar.maxValue = maxHealth;
            bossHealthBar.value = currentHealth;
        }

        if (!backgroundMusic) return;
        _backgroundAudioSource.clip = backgroundMusic;
        _backgroundAudioSource.Play();
    }

    public void TakeDamage(float damage)
    {
        if (_isReviving) return;

        currentHealth -= damage;
        if (currentHealth <= 0f)
        {
            currentHealth = 0f;
            _boxCollider.enabled = false;
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
        if (_backgroundAudioSource.isPlaying)
            _backgroundAudioSource.Stop();

        if (deathSound)
            _audioSource.PlayOneShot(deathSound);

        if (smokePrefab)
            Instantiate(smokePrefab, transform.position, Quaternion.identity);

        if (skyboxPhase2 && !_isPhase2)
        {
            RenderSettings.skybox = skyboxPhase2;
            DynamicGI.UpdateEnvironment();
        }

        if (bossLight)
            StartCoroutine(IncreaseLightIntensity());

        StartCoroutine(DeathSequence());
    }

    private IEnumerator IncreaseLightIntensity()
    {
        bossLight.enabled = true;
        const float duration = 5f;
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
        const float tiltDuration = 5f;
        const float sinkDuration = 5f;
        const float changeAppearanceDelay = 4f;
        const float riseDuration = 5f;

        Quaternion tiltedRotation =
            Quaternion.Euler(90f, _originalRotation.eulerAngles.y, _originalRotation.eulerAngles.z);
        yield return StartCoroutine(RotateOverTime(tiltedRotation, tiltDuration));
        yield return new WaitForSeconds(0.5f);

        Vector3 sinkTargetPosition = _originalPosition + Vector3.down * 500f;
        yield return StartCoroutine(MoveOverTime(sinkTargetPosition, sinkDuration));
        yield return new WaitForSeconds(changeAppearanceDelay);

        if (!_isPhase2)
        {
            if (bossRenderer && phase2Material)
                bossRenderer.material = phase2Material;

            yield return StartCoroutine(RotateOverTime(_originalRotation, tiltDuration));
            Vector3 riseTargetPosition = sinkTargetPosition + Vector3.up * 500f;
            yield return StartCoroutine(MoveOverTime(riseTargetPosition, riseDuration));
            Revive();
        }
        else
        {
            Destroy(gameObject);
            SceneManager.LoadScene(0);
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
        if (_isPhase2)
        {
            Destroy(gameObject);
            return;
        }

        _isPhase2 = true;
        maxHealth *= 10;
        currentHealth = maxHealth;

        if (bossHealthBar)
        {
            bossHealthBar.maxValue = maxHealth;
            bossHealthBar.value = currentHealth;
            bossHealthBar.gameObject.SetActive(true);
        }

        _boxCollider.enabled = true;
        _isReviving = false;
    }
}