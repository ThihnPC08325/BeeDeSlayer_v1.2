using UnityEngine;
using UnityEngine.UI;
using TMPro; // Nếu dùng TextMeshPro

public class LoadingZoneController : MonoBehaviour
{
    [Header("Zone Settings")]
    [SerializeField] private float zoneWidth = 5f;
    [SerializeField] private float zoneLength = 5f;
    [SerializeField] private float loadingTime = 120f;
    [SerializeField] private Color zoneColor = new Color(0, 1, 0, 0.3f);

    [Header("UI References")]
    [SerializeField] private GameObject progressBarContainer; // Container chứa UI
    [SerializeField] private Slider progressBar;
    [SerializeField] private TextMeshProUGUI percentageText; // Tùy chọn


    [Header("Effects")]
    [SerializeField] private AudioClip enterSound;
    [SerializeField] private AudioClip loadingCompleteSound;
    [SerializeField] private ParticleSystem loadingParticles;

    private float currentProgress = 0f;
    private bool playerInZone = false;

    private AudioSource audioSource;
    private Material zoneMaterial;

    void Awake()
    {
        // Khởi tạo components
        audioSource = gameObject.AddComponent<AudioSource>();
        zoneMaterial = GetComponent<Renderer>().material;
        
        // Thiết lập ban đầu
        UpdateZoneSize();
        InitializeUI();
    }

    void Start()
    {
        // Đảm bảo UI được ẩn khi bắt đầu
        if (progressBarContainer != null)
        {
            progressBarContainer.SetActive(false);
        }

        // Reset progress bar
        if (progressBar != null)
        {
            progressBar.value = 0;
        }
    }

    void Update()
    {
        if (playerInZone)
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
    }

    private void UpdateProgress()
    {
        currentProgress += Time.deltaTime / loadingTime;
        currentProgress = Mathf.Clamp01(currentProgress);

        if (progressBar != null)
        {
            progressBar.value = currentProgress;
        }

        // Cập nhật text phần trăm (nếu có)
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
            // Hiện UI khi player vào zone
            ShowProgressUI();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = false;
            // Reset và ẩn UI khi player rời zone
            HideAndResetProgress();
        }
    }
    private void ShowProgressUI()
    {
        if (progressBarContainer != null)
        {
            // Có thể thêm animation hiện UI ở đây
            progressBarContainer.SetActive(true);
        }
    }

    private void HideAndResetProgress()
    {
        if (progressBarContainer != null)
        {
            progressBarContainer.SetActive(false);
        }

        currentProgress = 0f;
        if (progressBar != null)
        {
            progressBar.value = 0;
        }
    }

    private void OnLoadingComplete()
    {
        if (loadingCompleteSound != null)
            audioSource.PlayOneShot(loadingCompleteSound);
            
        Debug.Log("Loading Complete! 🎉");
        // Thêm code xử lý khi hoàn thành ở đây
    }

    // Phương thức public để điều chỉnh từ Inspector hoặc script khác
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