using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    [SerializeField] private GameObject passwordPanel; // Panel nhập mật khẩu
    private bool isPanelOpen = false;
    [SerializeField] private NoteManager noteManager;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        passwordPanel.SetActive(false); // Bắt đầu ẩn panel
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            TogglePasswordPanel();
        }
    }

    void TogglePasswordPanel()
    {
        isPanelOpen = !isPanelOpen;
        passwordPanel.SetActive(isPanelOpen);

        Debug.Log(isPanelOpen ? "✅ Mở Panel!" : "❌ Đóng Panel!");

        if (isPanelOpen)
        {
            noteManager.ShowPass();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    // 🔹 Thêm hàm ShowPasswordPanel() để sửa lỗi trong NotePickup.cs
    public void ShowPasswordPanel()
    {
        isPanelOpen = true;
        passwordPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("📜 Đã mở Panel nhập mật khẩu!");
    }
}
