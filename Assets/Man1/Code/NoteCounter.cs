using UnityEngine;

[CreateAssetMenu(fileName = "NoteCounter", menuName = "Game/NoteCounter")]
public class NoteCounter : ScriptableObject
{
    private const string NoteCountKey = "CollectedNoteCount";

    [SerializeField] private int collectedNoteCount = 0;

    public int CollectedNoteCount => collectedNoteCount;

    private void OnEnable()
    {
#if UNITY_EDITOR
        // Chỉ reset trong Unity Editor khi nhấn nút Play
        if (!Application.isPlaying)
        {
            return; // Tránh reset khi chạy trong Editor nhưng không Play
        }
#endif

        ResetNoteCount(); // Reset chỉ khi bắt đầu Play
    }

    public void IncrementNoteCount()
    {
        collectedNoteCount++;
        PlayerPrefs.SetInt(NoteCountKey, collectedNoteCount); // Lưu giá trị mới
        Debug.Log("Note count incremented: " + collectedNoteCount);
    }

    public void ResetNoteCount()
    {
        collectedNoteCount = 0;
        PlayerPrefs.SetInt(NoteCountKey, collectedNoteCount); // Lưu giá trị reset
        Debug.Log("Note count reset to 0");
    }
}
