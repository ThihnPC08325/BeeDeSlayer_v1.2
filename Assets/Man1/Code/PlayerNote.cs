using UnityEngine;

public class PlayerNote : MonoBehaviour
{
    private NoteScript _activeNote;
    [SerializeField] private GameObject interactMessage;
    [SerializeField] private NoteData noteData;

    private void Start()
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

    private void Update()
    {
        if (!_activeNote || !Input.GetKeyDown(KeyCode.E)) return;
        Debug.Log("Nhấn E, mở note");
        _activeNote.ToggleNote();
        noteData.CollectNote(_activeNote.GetNoteID());
        _activeNote.gameObject.SetActive(false);
        interactMessage.SetActive(false); // Ẩn message sau khi nhặt note
    }

    private void OnTriggerEnter(Collider col)
    {
        if (!col.gameObject.CompareTag("Note") || !col.gameObject.TryGetComponent(out NoteScript note)) return;
        _activeNote = note;

        // Nếu note đã được nhặt, không hiển thị message
        if (noteData.IsNoteCollected(_activeNote.GetNoteID()))
        {
            _activeNote.gameObject.SetActive(false);
            return;
        }

        Debug.Log("Gần giấy note");
        interactMessage.SetActive(true);
    }

    private void OnTriggerExit(Collider col)
    {
        if (!col.gameObject.CompareTag("Note")) return;
        // Đảm bảo tắt note nếu đang mở
        if (_activeNote != null && _activeNote.GetNoteStatus())
        {
            Debug.Log("Đóng note khi rời khỏi");
            _activeNote.ToggleNote();
        }

        _activeNote = null;
        interactMessage.SetActive(false); // Luôn ẩn message khi rời khỏi
        Debug.Log("Rời khỏi vùng va chạm, tắt message.");
    }
}
