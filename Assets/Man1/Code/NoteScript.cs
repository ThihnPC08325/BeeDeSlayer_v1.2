using UnityEngine;
using System.Collections.Generic;

public class NoteScript : MonoBehaviour
{
    private bool noteStatus = false;
    public GameObject note;

    [SerializeField] private string noteID;
    [SerializeField] private NoteData noteData; // Tham chiếu tới NoteData ScriptableObject
    [SerializeField] private List<Transform> possiblePositions;
    [SerializeField] private NoteCounter noteCounter; // Tham chiếu tới NoteCounter ScriptableObject

    void Start()
    {
        // Kiểm tra nếu note đã được nhặt trước đó thông qua NoteData
        if (noteData.IsNoteCollected(noteID))
        {
            gameObject.SetActive(false); // Ẩn nếu đã nhặt
        }
        else
        {
            ActivateNote(); // Đặt vị trí ngẫu nhiên nếu chưa nhặt
        }
    }

    public void ToggleNote()
    {
        noteStatus = !noteStatus;
        note.SetActive(noteStatus);
        Debug.Log("Trạng thái note: " + noteStatus);

        // Khi bật ghi chú, kiểm tra nếu nó chưa được thu thập thì thu thập nó
        if (noteStatus && !noteData.IsNoteCollected(noteID))
        {
            CollectNote();
        }
    }

    public bool GetNoteStatus()
    {
        return noteStatus;
    }

    public string GetNoteID()
    {
        return noteID;
    }

    public void ActivateNote()
    {
        gameObject.SetActive(true);

        if (possiblePositions != null && possiblePositions.Count > 0)
        {
            int randomIndex = Random.Range(0, possiblePositions.Count);
            transform.position = possiblePositions[randomIndex].position;

            // Kích hoạt lại collider để chuẩn bị cho việc nhặt ghi chú
            Collider noteCollider = GetComponent<Collider>();
            if (noteCollider != null)
            {
                noteCollider.enabled = false;
                noteCollider.enabled = true;
            }
        }
        else
        {
            Debug.LogWarning("possiblePositions is empty or not assigned for note: " + noteID);
        }
    }

    private void CollectNote()
    {
        // Gọi đúng hàm trong NoteData để lưu trạng thái đã thu thập
        noteData.CollectNote(noteID);

        // Tăng số lượng ghi chú đã nhặt trong NoteCounter
        if (noteCounter != null)
        {
            noteCounter.IncrementNoteCount();
            Debug.Log("Note được nhặt: " + noteID + ". Tổng số ghi chú đã nhặt: " + noteCounter.CollectedNoteCount);
        }
        else
        {
            Debug.LogWarning("noteCounter chưa được gán trong Inspector!");
        }

        // Ẩn ghi chú sau khi đã nhặt
        gameObject.SetActive(false);
    }
}
