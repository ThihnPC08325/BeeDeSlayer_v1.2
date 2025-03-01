using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class LoadingZoneController : MonoBehaviour
{
    [Header("Zone Settings")]
    [SerializeField] private float loadingTime = 120f;

    [Header("UI References")]
    [SerializeField] private GameObject progressBarContainer;
    [SerializeField] private Slider progressBar;
    [SerializeField] private TextMeshProUGUI percentageText;
    [SerializeField] private TextMeshProUGUI completionText;

    [Header("Effects")]
    [SerializeField] private AudioClip enterSound;
    [SerializeField] private AudioClip loadingCompleteSound;

    public int LaptopsLoaded { get; private set; } = 0;
    private static int _totalLaptopsLoaded = 4; // Theo dõi tổng số laptop

    [SerializeField] private int laptopID; // ID riêng của laptop này
    private static int _nextLaptopID = 1; // Biến static để cấp phát ID tự động

    private float _currentProgress = 0f;
    private bool _playerInZone = false;
    private bool _isZoneCompleted = false;
    private AudioSource _audioSource;

    void Awake()
    {
        _audioSource = gameObject.AddComponent<AudioSource>();
        InitializeUI();

        if (laptopID == 0)
        {
            laptopID = _nextLaptopID;
            _nextLaptopID++;
        }
    }

    void Start()
    {
        ResetAllZones(); // Reset tất cả zone khi game khởi động
    }

    void Update()
    {
        if (_playerInZone && !_isZoneCompleted)
        {
            UpdateProgress();
        }
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
        if (_isZoneCompleted) return;

        _currentProgress += Time.deltaTime / loadingTime;
        _currentProgress = Mathf.Clamp01(_currentProgress);

        if (progressBar)
        {
            progressBar.value = _currentProgress;
        }

        if (percentageText)
        {
            percentageText.text = $"{(_currentProgress * 100):F1}%";
        }

        if (_currentProgress >= 1f)
        {
            OnLoadingComplete();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        _playerInZone = true;

        if (!_isZoneCompleted)
        {
            if (enterSound != null)
            {
                _audioSource.PlayOneShot(enterSound);
            }
            ShowProgressUI();
        }
        else
        {
            ShowCompletionUI();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        _playerInZone = false;
        if (!_isZoneCompleted)
        {
            HideAndResetProgress();
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

        if (_isZoneCompleted) return;
        _currentProgress = 0f;
        if (progressBar != null)
        {
            progressBar.value = 0;
        }
    }

    private void ShowCompletionUI()
    {
        if (completionText)
        {
            completionText.gameObject.SetActive(true);
            completionText.text = "Đã hoàn thành";
        }
        if (progressBarContainer)
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
        if (loadingCompleteSound)
        {
            _audioSource.PlayOneShot(loadingCompleteSound);
        }

     

        _isZoneCompleted = true;
        LaptopsLoaded++; // Cập nhật số laptop đã tải
        _totalLaptopsLoaded++; // Cập nhật tổng số laptop đã tải trên toàn bộ game

        // 🟢 Log số laptop đã tải xong
        Debug.Log($"Đã tải xong laptop ID {laptopID}");

        _isZoneCompleted = true;
        ShowCompletionUI();
        HideProgressUI();

        Debug.Log("Loading Complete! 🎉");

        // Ẩn dòng chữ "Đã hoàn thành" sau 5 giây
        StartCoroutine(HideCompletionTextAfterDelay(5f));
    }

    private void HideProgressUI()
    {
        if (progressBarContainer)
        {
            progressBarContainer.SetActive(false);
        }
    }

    private IEnumerator HideCompletionTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (completionText)
        {
            completionText.gameObject.SetActive(false);
        }
    }

    // Reset trạng thái hoàn thành của zone
    public void ResetZone()
    {
        _currentProgress = 0f;
        _isZoneCompleted = false;

        if (progressBar != null)
        {
            progressBar.value = 0;
        }

        HideCompletionUI();
    }

    // Reset tất cả các zone khi game khởi động
    public static void ResetAllZones()
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
}