using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EndGameUI : MonoBehaviour
{
    [SerializeField] private Text congratulationText; // UI Text hiển thị chúc mừng
    [SerializeField] private string[] congratulationMessages; // Mảng các đoạn văn cần hiển thị
    [SerializeField] private float typingSpeed = 0.1f; // Tốc độ gõ chữ

    private void Start()
    {
        StartCoroutine(DisplayMessages());
    }

    private IEnumerator DisplayMessages()
    {
        // Lặp qua tất cả các đoạn văn
        foreach (string message in congratulationMessages)
        {
            // Hiển thị từng đoạn văn
            yield return StartCoroutine(TypeMessage(message));
            // Dừng một chút trước khi chuyển sang đoạn văn tiếp theo
            yield return new WaitForSeconds(1f); // Tùy chỉnh thời gian nghỉ giữa các đoạn văn
        }
    }

    private IEnumerator TypeMessage(string message)
    {
        congratulationText.text = ""; // Xóa trước khi gõ
        foreach (char letter in message.ToCharArray())
        {
            congratulationText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }
}
