﻿using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private NoteCounter noteCounter;

    private static bool hasGameStarted = false;

    private void Awake()
    {
        if (!hasGameStarted)
        {
            hasGameStarted = true;
            noteCounter.ResetNoteCount(); // Reset khi trò chơi bắt đầu mới
            Debug.Log("Reset NoteCounter khi bắt đầu trò chơi.");
        }
        else
        {
            Debug.Log("Không reset NoteCounter khi quay lại scene.");
        }
    }

    public static void RestartGame()
    {
        hasGameStarted = false; // Đặt lại trạng thái
        Debug.Log("Restarting game...");
    }
}