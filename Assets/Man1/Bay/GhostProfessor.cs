using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GhostProfessor : MonoBehaviour
{
    [Header("Gợi ý theo số giấy thu thập")]
    public string[] hints0; // Chưa nhặt tờ nào
    public string[] hints1; // Nhặt 1 tờ
    public string[] hints2; // Nhặt 2 tờ
    public string[] hints3; // Nhặt 3 tờ
    public string[] hints4; // Đủ 4 tờ

    [Header("UI Elements")]
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

        string hint = GetRandomHint();
        _hintText.text = $"{hint}";

        hintUI.SetActive(true);

        if (noteCounter != null && noteCounter.CollectedNoteCount >= 4)
        {
            Time.timeScale = 0f;
            passwordPanel.SetActive(true);
            hintUI.SetActive(false);
            EnableCursor();
        }
    }

    private void HideHint()
    {
        hintUI.SetActive(false);
        passwordPanel.SetActive(false);
        DisableCursor();
        Time.timeScale = 1f;
    }

    private void LookAtPlayer()
    {
        if (!_player) return;
        Vector3 direction = (_player.position - transform.position).normalized;
        direction.y = 0;
        transform.forward = direction;
    }

    private string GetRandomHint()
    {
        int collected = noteCounter.CollectedNoteCount;

        if (collected >= 4) return hints4[Random.Range(0, hints4.Length)];
        if (collected == 3) return hints3[Random.Range(0, hints3.Length)];
        if (collected == 2) return hints2[Random.Range(0, hints2.Length)];
        if (collected == 1) return hints1[Random.Range(0, hints1.Length)];
        return hints0[Random.Range(0, hints0.Length)];
    }

    public void CheckPassword()
    {
        if (passwordInput.text == correctPassword)
        {
            sceneChanger.LoadTargetScene();
            Time.timeScale = 1f;
        }
        else
        {
            _hintText.text = "👻 Ghost: Sai mật khẩu! Thử lại đi.";
        }
    }

    private void EnableCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
    }

    private void DisableCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
    }
}