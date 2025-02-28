using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class NPCDialogue : MonoBehaviour
{
    [Header("References")]
    public LoadingZoneController loadingZone; // Tham chiếu đến LoadingZoneController
    public TextMeshProUGUI dialogueText; // Tham chiếu đến UI hiển thị hội thoại
    public TextMeshProUGUI interactText; // Hiển thị "Nhấn E để nói chuyện"

    [Header("Dialogue")]
    [TextArea(3, 5)]
    public string[] dialogueGuide = {
        "Bạn ơi, hãy giúp tôi với, tôi đã bị con quái vật kia biến thành cái Tivi như thế này.",
        "Hãy giúp tôi bằng cách tìm 4 cái laptop xung quanh thế giới vô định này để tải lên những dữ liệu mà quái vật kia đánh cắp.",
        "Sau khi hoàn thành, hãy quay lại tìm tôi... còn một việc quan trọng nữa tôi cần bạn giúp."
    };

    [TextArea(3, 5)]
    public string[] dialogueComplete = {
        "Cảm ơn bạn, cuối cùng hãy giúp tôi điều cuối cùng...",
        "Hãy đánh bại con quái vật kia đi, tôi sắp chịu không nổi nữa rồi...",
        "Hãy.... giúp tôi....."
    };

    private bool isPlayerNear = false; // Kiểm tra người chơi có gần không
    private bool hasSeenGuide = false; // Kiểm tra đã xem hết hướng dẫn chưa
    private bool hasTaskCompleted = false; // Kiểm tra nhiệm vụ hoàn thành chưa
    private int currentDialogueIndex = 0; // Chỉ mục câu hiện tại trong hướng dẫn hoặc nhiệm vụ hoàn thành
    private bool isTyping = false; // Kiểm tra có đang chạy hiệu ứng đánh chữ không
    private bool isCompleteDialogue = false; // Kiểm tra đang hiển thị đoạn kết hay không

    void Start()
    {
        dialogueText.gameObject.SetActive(false); // Ẩn hội thoại lúc ban đầu
    }

    void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
            interactText.gameObject.SetActive(true); // Hiện "Nhấn E để nói chuyện"
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            interactText.gameObject.SetActive(false); // Ẩn hướng dẫn khi rời khỏi NPC
            dialogueText.gameObject.SetActive(false); // Ẩn hội thoại khi rời NPC
        }
    }

    public void Interact()
    {
        dialogueText.gameObject.SetActive(true); // Hiện hộp thoại khi nói chuyện

        if (!hasSeenGuide) // Nếu chưa xem hết hướng dẫn
        {
            if (!isTyping) // Nếu không đang chạy hiệu ứng chữ, hiển thị câu tiếp theo
            {
                if (currentDialogueIndex < dialogueGuide.Length)
                {
                    StartCoroutine(TypeText(dialogueGuide[currentDialogueIndex]));
                    currentDialogueIndex++;
                }
                else
                {
                    hasSeenGuide = true; // Đánh dấu đã xem hết hướng dẫn
                    currentDialogueIndex = 0; // Reset chỉ mục để dùng cho đoạn kết
                    ShowLaptopProgress();
                }
            }
        }
        else if (hasTaskCompleted) // Nếu nhiệm vụ hoàn thành, hiển thị đoạn kết
        {
            if (!isTyping)
            {
                if (currentDialogueIndex < dialogueComplete.Length)
                {
                    StartCoroutine(TypeText(dialogueComplete[currentDialogueIndex]));
                    currentDialogueIndex++;
                }
                else
                {
                    StartCoroutine(LoadNewSceneAfterDelay(5f)); // Sau khi đọc xong, load map mới
                }
            }
        }
        else
        {
            ShowLaptopProgress();
        }
    }

    private IEnumerator TypeText(string text)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char letter in text.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.05f); // Tốc độ đánh chữ
        }

        isTyping = false;
    }

    private void ShowLaptopProgress()
    {
        int totalLaptopsLoaded = GetTotalLaptopsLoaded(); // Lấy tổng số laptop đã tải

        Debug.Log($"📌 Người chơi đã tải {totalLaptopsLoaded}/4 laptop"); // Kiểm tra trong console

        if (totalLaptopsLoaded >= 4) // Nếu đã đủ 4 laptop
        {
            hasTaskCompleted = true;
            isCompleteDialogue = true;
            currentDialogueIndex = 0; // Reset index để bắt đầu hội thoại kết
            Interact(); // Bắt đầu hiển thị đoạn kết
        }
        else
        {
            StartCoroutine(TypeText($"Bạn mới tải được {totalLaptopsLoaded}/4 laptop. Cần đủ 4 laptop để tiếp tục!"));
        }

        StartCoroutine(HideDialogueAfterDelay(5f)); // Tự động ẩn hộp thoại sau 5s
    }

    private int GetTotalLaptopsLoaded()
    {
        int total = 0;
        LoadingZoneController[] allZones = FindObjectsOfType<LoadingZoneController>();

        foreach (var zone in allZones)
        {
            total += zone.LaptopsLoaded;
        }

        return total;
    }

    private IEnumerator LoadNewSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("BossMap2"); // Thay tên scene bạn muốn
    }

    private IEnumerator HideDialogueAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        dialogueText.gameObject.SetActive(false);
    }
}
