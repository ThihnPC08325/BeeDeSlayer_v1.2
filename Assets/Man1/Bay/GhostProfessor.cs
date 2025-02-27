using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GhostProfessor : MonoBehaviour
{
    public string[] hints;
    public GameObject hintUI;
    public GameObject passwordPanel;
    public TMP_InputField passwordInput;
    public Button submitButton;
    public string correctPassword = "1234";

    private TextMeshProUGUI _hintText;
    private bool _isBeeNearby = false;
    private Transform _player;

    public NoteCounter noteCounter;
    public SceneChanger sceneChanger;

    private void Start()
    {
        _hintText = hintUI.GetComponentInChildren<TextMeshProUGUI>();
        hintUI.SetActive(false);
        passwordPanel.SetActive(false);
        _player = GameObject.FindGameObjectWithTag("Player").transform;

        if (submitButton != null)
        {
            submitButton.onClick.AddListener(CheckPassword);
        }
    }

    private void Update()
    {
        if (_isBeeNearby)
        {
            LookAtPlayer();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        _isBeeNearby = true;
        ShowHint();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        _isBeeNearby = false;
        HideHint();
    }

    private void ShowHint()
    {
        if (!_isBeeNearby) return;

        string hint = hints[Random.Range(0, hints.Length)];
        _hintText.text = $"👻 Ghost: {hint}";

        hintUI.SetActive(true);

        if (noteCounter != null && noteCounter.CollectedNoteCount >= 4)
        {
            passwordPanel.SetActive(true);
            EnableCursor(); // Hiển thị con trỏ chuột khi nhập mật khẩu
        }
    }

    private void HideHint()
    {
        hintUI.SetActive(false);
        passwordPanel.SetActive(false);
        DisableCursor(); // Ẩn con trỏ chuột khi thoát
    }

    private void LookAtPlayer()
    {
        if (!_player) return;
        Vector3 direction = (_player.position - transform.position).normalized;
        direction.y = 0;
        transform.forward = direction;
    }

    public void CheckPassword()
    {
        if (passwordInput.text == correctPassword)
        {
            sceneChanger.LoadTargetScene();
        }
        else
        {
            _hintText.text = "👻 Ghost: Sai mật khẩu! Thử lại đi.";
        }
    }

    private void EnableCursor()
    {
        Cursor.lockState = CursorLockMode.None; // Cho phép di chuyển chuột
        Cursor.visible = true; // Hiện con trỏ chuột
    }

    private void DisableCursor()
    {
        Cursor.lockState = CursorLockMode.Locked; // Khóa con trỏ vào màn hình
        Cursor.visible = false; // Ẩn con trỏ chuột
    }
}
