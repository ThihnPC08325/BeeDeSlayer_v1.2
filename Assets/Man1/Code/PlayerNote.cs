using UnityEngine;

public class PlayerNote : MonoBehaviour
{
    private NoteScript activeNote;
    [SerializeField] private GameObject interactMessage;
    [SerializeField] private NoteData noteData;

    void Start()
    {
        if (interactMessage == null)
        {
            Debug.LogError("InteractMessage chưa được gán trong Inspector!");
        }
        else
        {
            interactMessage.SetActive(false); // Đảm bảo message ban đầu ẩn
        }
    }

    void Update()
    {
        if (activeNote && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Nhấn E, mở note");
            activeNote.ToggleNote();
            noteData.CollectNote(activeNote.GetNoteID());
            activeNote.gameObject.SetActive(false);
            interactMessage.SetActive(false); // Ẩn message sau khi nhặt note
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Note") && col.gameObject.TryGetComponent(out NoteScript note))
        {
            activeNote = note;

            // Nếu note đã được nhặt, không hiển thị message
            if (noteData.IsNoteCollected(activeNote.GetNoteID()))
            {
                activeNote.gameObject.SetActive(false);
                return;
            }

            Debug.Log("Gần giấy note");
            interactMessage.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.gameObject.CompareTag("Note"))
        {
            // Đảm bảo tắt note nếu đang mở
            if (activeNote != null && activeNote.GetNoteStatus())
            {
                Debug.Log("Đóng note khi rời khỏi");
                activeNote.ToggleNote();
            }

            activeNote = null;
            interactMessage.SetActive(false); // Luôn ẩn message khi rời khỏi
            Debug.Log("Rời khỏi vùng va chạm, tắt message.");
        }
    }
}
