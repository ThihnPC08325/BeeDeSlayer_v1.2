using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public GameObject settingsMenuUI;
    public Slider musicVolumeSlider;
    public Slider mouseSensitivitySlider;
    public Toggle fullscreenToggle;

    public Button resumeButton;
    public Button settingsButton;
    public Button mainMenuButton;
    public Button quitButton;
    public Button backButton;

    private bool _isPaused = false;

    private void Start()
    {
        pauseMenuUI.SetActive(false);
        settingsMenuUI.SetActive(false);

        resumeButton.onClick.AddListener(ResumeGame);
        settingsButton.onClick.AddListener(OpenSettings);
        mainMenuButton.onClick.AddListener(GoToMainMenu);
        quitButton.onClick.AddListener(QuitGame);
        backButton.onClick.AddListener(CloseSettings);

        fullscreenToggle.onValueChanged.AddListener(ToggleFullscreen);
        musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        mouseSensitivitySlider.onValueChanged.AddListener(SetMouseSensitivity);
    }

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Escape)) return;
        if (_isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    private void PauseGame()
    {
        Debug.Log("PauseGame Called"); // Kiểm tra xem nó có chạy không
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        _isPaused = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        EventSystem.current.SetSelectedGameObject(null); // Reset selection
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        settingsMenuUI.SetActive(false);
        Time.timeScale = 1f;
        _isPaused = false;

        // Ẩn con trỏ khi quay lại game
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OpenSettings()
    {
        pauseMenuUI.SetActive(false);
        settingsMenuUI.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsMenuUI.SetActive(false);
        pauseMenuUI.SetActive(true);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ToggleFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetMusicVolume(float volume)
    {
        AudioListener.volume = volume;
    }

    public void SetMouseSensitivity(float sensitivity)
    {
        Debug.Log("Mouse Sensitivity: " + sensitivity);
        // Thêm xử lý độ nhạy chuột ở đây nếu có
    }
}