using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI; // Kéo thả panel menu vào đây
    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f; // Dừng toàn bộ game
        isPaused = true;
        Cursor.lockState = CursorLockMode.None; // Hiện con trỏ chuột
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f; // Tiếp tục game
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked; // Ẩn con trỏ chuột
        Cursor.visible = false;
    }

    public void QuitGame()
    {
        Application.Quit(); // Thoát game
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
