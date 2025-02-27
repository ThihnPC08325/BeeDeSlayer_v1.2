using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PasswordUI : MonoBehaviour
{
    public static PasswordUI Instance { get; private set; }

    [SerializeField] private GameObject passwordPanel; // Panel nhập mật khẩu
    [SerializeField] private Transform noteContainer; // Vùng hiển thị note
    [SerializeField] private GameObject notePrefab; // Prefab giấy note
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button confirmButton; // 🔹 Nút xác nhận mật khẩu
    [SerializeField] private NoteManager noteManager;
    private string correctPassword = "GA19301";

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
        passwordPanel.SetActive(false); // Ẩn panel ban đầu

        if (confirmButton != null)
        {
            confirmButton.onClick.AddListener(CheckPassword); // 🔹 Đảm bảo nút có sự kiện onClick
        }
        else
        {
            Debug.LogError("⚠️ Nút Confirm Button chưa được gán trong Inspector!");
        }
    }

    public void ShowPasswordPanel()
    {
        passwordPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        DisplayCollectedNotes();
    }

    void DisplayCollectedNotes()
    {
        foreach (Transform child in noteContainer)
        {
            Destroy(child.gameObject);
        }

        List<string> collectedNotes = NoteManager.Instance.GetCollectedNotes();
        Debug.Log($"📜 Hiển thị {collectedNotes.Count} giấy note trên UI");

        foreach (string note in collectedNotes)
        {
            GameObject newNote = Instantiate(notePrefab, noteContainer);
            newNote.GetComponentInChildren<TextMeshProUGUI>().text = note;
        }
    }

    public void CheckPassword()
    {
        string enteredPassword = inputField.text.Trim();

        if (enteredPassword.ToUpper() == correctPassword.ToUpper())
        {
            Debug.Log("✅ Mật khẩu đúng! Chuyển scene...");
            passwordPanel.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

             SceneManager.LoadScene("Man1.5");
        }
        else
        {
            Debug.Log("❌ Sai mật khẩu! Hãy nhập lại.");
            inputField.text = "";
            inputField.ActivateInputField();
        }
    }
}