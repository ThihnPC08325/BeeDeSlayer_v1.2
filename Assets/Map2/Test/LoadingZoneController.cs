using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class LoadingZoneController : MonoBehaviour
{
    [Header("Zone Settings")]
    [SerializeField] private float zoneWidth = 5f;
    [SerializeField] private float zoneLength = 5f;
    [SerializeField] private float loadingTime = 120f;
    [SerializeField] private Color zoneColor = new Color(0, 1, 0, 0.3f);

    [Header("UI References")]
    [SerializeField] private GameObject progressBarContainer;
    [SerializeField] private Slider progressBar;
    [SerializeField] private TextMeshProUGUI percentageText;
    [SerializeField] private TextMeshProUGUI completionText;

    [Header("Effects")]
    [SerializeField] private AudioClip enterSound;
    [SerializeField] private AudioClip loadingCompleteSound;
    [SerializeField] private ParticleSystem loadingParticles;

    private float currentProgress = 0f;
    private bool playerInZone = false;
    private bool isZoneCompleted = false;
    private AudioSource audioSource;
    private Material zoneMaterial;

    void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        zoneMaterial = GetComponent<Renderer>().material;

        UpdateZoneSize();
        InitializeUI();
    }

    void Start()
    {
        ResetAllZones(); // Reset t?t c? zone khi game kh?i ??ng
    }

    void Update()
    {
        if (playerInZone && !isZoneCompleted)
        {
            UpdateProgress();
        }
    }

    private void UpdateZoneSize()
    {
        transform.localScale = new Vector3(zoneWidth, 0.1f, zoneLength);
        zoneMaterial.color = zoneColor;
    }

    private void InitializeUI()
    {
        if (progressBar != null)
        {
            progressBar.value = 0;
            progressBar.maxValue = 1;
        }

        if (completionText != null)
        {
            completionText.gameObject.SetActive(false);
        }
    }

    private void UpdateProgress()
    {
        if (isZoneCompleted) return;

        currentProgress += Time.deltaTime / loadingTime;
        currentProgress = Mathf.Clamp01(currentProgress);

        if (progressBar != null)
        {
            progressBar.value = currentProgress;
        }

        if (percentageText != null)
        {
            percentageText.text = $"{(currentProgress * 100):F1}%";
        }

        if (currentProgress >= 1f)
        {
            OnLoadingComplete();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = true;

            if (!isZoneCompleted)
            {
                if (enterSound != null)
                {
                    audioSource.PlayOneShot(enterSound);
                }
                ShowProgressUI();
            }
            else
            {
                ShowCompletionUI();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = false;
            if (!isZoneCompleted)
            {
                HideAndResetProgress();
            }
        }
    }

    private void ShowProgressUI()
    {
        if (progressBarContainer != null)
        {
            progressBarContainer.SetActive(true);
        }
        if (completionText != null)
        {
            completionText.gameObject.SetActive(false);
        }
    }

    private void HideAndResetProgress()
    {
        if (progressBarContainer != null)
        {
            progressBarContainer.SetActive(false);
        }

        if (!isZoneCompleted)
        {
            currentProgress = 0f;
            if (progressBar != null)
            {
                progressBar.value = 0;
            }
        }
    }

    private void ShowCompletionUI()
    {
        if (completionText != null)
        {
            completionText.gameObject.SetActive(true);
            completionText.text = "?ã hoàn thành";
        }
        if (progressBarContainer != null)
        {
            progressBarContainer.SetActive(false);
        }
    }

    private void HideCompletionUI()
    {
        if (completionText != null)
        {
            completionText.gameObject.SetActive(false);
        }
    }

    private void OnLoadingComplete()
    {
        if (loadingCompleteSound != null)
        {
            audioSource.PlayOneShot(loadingCompleteSound);
        }

        if (loadingParticles != null)
        {
            loadingParticles.Play();
        }

        isZoneCompleted = true;
        ShowCompletionUI();
        HideProgressUI();

        Debug.Log("Loading Complete! ??");

        // ?n dòng ch? "?ã hoàn thành" sau 5 giây
        StartCoroutine(HideCompletionTextAfterDelay(5f));
    }

    private void HideProgressUI()
    {
        if (progressBarContainer != null)
        {
            progressBarContainer.SetActive(false);
        }
    }

    private IEnumerator HideCompletionTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (completionText != null)
        {
            completionText.gameObject.SetActive(false);
        }
    }

    // ?? Reset tr?ng thái hoàn thành c?a zone
    public void ResetZone()
    {
        currentProgress = 0f;
        isZoneCompleted = false;

        if (progressBar != null)
        {
            progressBar.value = 0;
        }

        HideCompletionUI();
    }

    // ?? Reset t?t c? các zone khi game kh?i ??ng (không c?n GameManager)
    public void ResetAllZones()
    {
        LoadingZoneController[] allZones = FindObjectsOfType<LoadingZoneController>();
        foreach (var zone in allZones)
        {
            zone.ResetZone();
        }
    }

    public void SetLoadingTime(float newTime)
    {
        loadingTime = newTime;
    }

    public void SetZoneSize(float width, float length)
    {
        zoneWidth = width;
        zoneLength = length;
        UpdateZoneSize();
    }
}
