using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NoteManager : MonoBehaviour
{
    public static NoteManager Instance { get; private set; }
    [SerializeField] private PasswordData passwordData;
    [SerializeField] private TextMeshProUGUI passWordText;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Không bị xóa khi chuyển scene
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool AddNote(string note)
    {
        if (!passwordData.collectedNotes.Contains(note)) // Kiểm tra nếu chưa nhặt trước đó
        {
            passwordData.collectedNotes.Add(note);
            return true;
        }
        return false;
    }

    public List<string> GetCollectedNotes()
    {
        return passwordData.collectedNotes;
    }

    public int GetNoteCount()
    {
        return passwordData.collectedNotes.Count;
    }

    public void ShowPass()
    {
        passWordText.text = "Password: ";
        int index = 0;
        string pass = "";
        foreach (var note in passwordData.collectedNotes)
        {
            if (index < passwordData.passWord.Count) // Kiểm tra tránh lỗi vượt quá danh sách
            {
                passWordText.text += passwordData.passWord[index] + " ";
            }
            index++;
        }
    }
}
