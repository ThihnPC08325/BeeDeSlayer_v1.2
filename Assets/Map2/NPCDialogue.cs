using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System.Linq;

public class NPCDialogue : MonoBehaviour
{
    [Header("References")] [SerializeField]
    private LoadingZoneController loadingZone; // Tham chiếu đến LoadingZoneController

    [SerializeField] private TextMeshProUGUI dialogueText; // Tham chiếu đến UI hiển thị hội thoại
    [SerializeField] private TextMeshProUGUI interactText; // Hiển thị "Nhấn E để nói chuyện"

    [Header("Dialogue")] [TextArea(3, 5)] [SerializeField]
    private string[] dialogueGuide =
    {
        "Bạn ơi, hãy giúp tôi với, tôi đã bị con quái vật kia biến thành cái Tivi như thế này.",
        "Hãy giúp tôi bằng cách tìm 4 cái laptop xung quanh thế giới vô định này để tải lên những dữ liệu mà quái vật kia đánh cắp.",
        "Sau khi hoàn thành, hãy quay lại tìm tôi... còn một việc quan trọng nữa tôi cần bạn giúp."
    };

    [TextArea(3, 5)] [SerializeField] private string[] dialogueComplete =
    {
        "Cảm ơn bạn, cuối cùng hãy giúp tôi điều cuối cùng...",
        "Hãy đánh bại con quái vật kia đi, tôi sắp chịu không nổi nữa rồi...",
        "Hãy.... giúp tôi....."
    };

    private bool _isPlayerNear = false; // Kiểm tra người chơi có gần không
    private bool _hasSeenGuide = false; // Kiểm tra đã xem hết hướng dẫn chưa
    private bool _hasTaskCompleted = false; // Kiểm tra nhiệm vụ hoàn thành chưa
    private int _currentDialogueIndex = 0; // Chỉ mục câu hiện tại trong hướng dẫn hoặc nhiệm vụ hoàn thành
    private bool _isTyping = false; // Kiểm tra có đang chạy hiệu ứng đánh chữ không
    private bool _isCompleteDialogue = false; // Kiểm tra đang hiển thị đoạn kết hay không

    void Start()
    {
        dialogueText.gameObject.SetActive(false); // Ẩn hội thoại lúc ban đầu
    }

    void Update()
    {
        if (_isPlayerNear && Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        _isPlayerNear = true;
        interactText.gameObject.SetActive(true); // Hiện "Nhấn E để nói chuyện"
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        _isPlayerNear = false;
        interactText.gameObject.SetActive(false); // Ẩn hướng dẫn khi rời khỏi NPC
        dialogueText.gameObject.SetActive(false); // Ẩn hội thoại khi rời NPC
    }

    private void Interact()
    {
        dialogueText.gameObject.SetActive(true); // Hiện hộp thoại khi nói chuyện

        if (_isTyping) return; // Nếu đang chạy hiệu ứng chữ, không thực hiện thêm hành động

        if (!_hasSeenGuide)
        {
            DisplayDialogue(dialogueGuide, () =>
            {
                _hasSeenGuide = true;
                _currentDialogueIndex = 0; // Reset chỉ mục
                ShowLaptopProgress(); // Hiển thị tiến độ thu thập laptop
            });
        }
        else if (_hasTaskCompleted)
        {
            DisplayDialogue(dialogueComplete, () =>
            {
                StartCoroutine(LoadNewSceneAfterDelay(5f)); // Chuyển sang màn mới sau khi hoàn tất
            });
        }
        else
        {
            ShowLaptopProgress(); // Nếu không có gì, hiển thị tiến độ thu thập laptop
        }
    }

    private void DisplayDialogue(string[] dialogues, System.Action onComplete)
    {
        if (_currentDialogueIndex < dialogues.Length)
        {
            StartCoroutine(TypeText(dialogues[_currentDialogueIndex]));
            _currentDialogueIndex++;
        }
        else
        {
            onComplete?.Invoke(); // Gọi callback khi hoàn thành
        }
    }

    private IEnumerator TypeText(string text)
    {
        _isTyping = true;
        dialogueText.text = "";

        foreach (char letter in text.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.05f); // Tốc độ đánh chữ
        }

        _isTyping = false;
    }

    private void ShowLaptopProgress()
    {
        int totalLaptopsLoaded = GetTotalLaptopsLoaded(); // Lấy tổng số laptop đã tải

        Debug.Log($"📌 Người chơi đã tải {totalLaptopsLoaded}/4 laptop"); // Kiểm tra trong console

        if (totalLaptopsLoaded >= 4) // Nếu đã đủ 4 laptop
        {
            _hasTaskCompleted = true;
            _isCompleteDialogue = true;
            _currentDialogueIndex = 0; // Reset index để bắt đầu hội thoại kết
            Interact(); // Bắt đầu hiển thị đoạn kết
        }
        else
        {
            StartCoroutine(TypeText($"Bạn mới tải được {totalLaptopsLoaded}/4 laptop. Cần đủ 4 laptop để tiếp tục!"));
        }

        StartCoroutine(HideDialogueAfterDelay(5f)); // Tự động ẩn hộp thoại sau 5s
    }

    private static int GetTotalLaptopsLoaded()
    {
        LoadingZoneController[] allZones = FindObjectsOfType<LoadingZoneController>();

        return allZones.Sum(zone => zone.LaptopsLoaded);
    }

    private static IEnumerator LoadNewSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("BossMap2");
    }

    private IEnumerator HideDialogueAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        dialogueText.gameObject.SetActive(false);
    }
}