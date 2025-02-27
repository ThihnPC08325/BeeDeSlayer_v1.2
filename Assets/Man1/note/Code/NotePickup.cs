using UnityEngine;

public class NotePickup : MonoBehaviour
{
    public string noteValue; // Giá trị của giấy note
    private float floatSpeed = 1.5f;
    private float rotationSpeed = 50f;
    private Vector3 startPos;
    [SerializeField] PasswordData passwordData;
    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
        float floatOffset = Mathf.Sin(Time.time * floatSpeed) * 0.2f;
        transform.position = startPos + new Vector3(0, floatOffset, 0);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (NoteManager.Instance == null)
            {
                Debug.LogError("❌ NoteManager.Instance không tồn tại!");
                return;
            }

            if (UIManager.Instance == null)
            {
                Debug.LogError("❌ UIManager.Instance không tồn tại!");
                return;
            }

            // Kiểm tra nếu note chưa được nhặt
            if (NoteManager.Instance.AddNote(noteValue))
            {
                Debug.Log($"📜 Nhặt giấy note: {noteValue} ({NoteManager.Instance.GetNoteCount()}/4)");

                // Nếu đã nhặt đủ 4 note, hiển thị panel nhập mật khẩu
                if (NoteManager.Instance.GetNoteCount() >= 4)
                {
                    UIManager.Instance.ShowPasswordPanel();
                }
                passwordData.passWord.Add(noteValue);
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("⚠️ Note này đã được nhặt trước đó!");
            }
        }
    }
}
