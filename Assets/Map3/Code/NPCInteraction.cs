using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NPCInteraction : MonoBehaviour
{
    [SerializeField] private KeyCode interactionKey = KeyCode.F;
    [SerializeField] private GameObject dialogueUI;
    [SerializeField] private TMP_Text dialogueText;    // Text hiển thị nội dung hội thoại
    [SerializeField] private ItemDropManager itemDropManager;
    [SerializeField] private Transform dropPosition;

    private bool isPlayerNear = false;
    private bool bossDefeated = false; // Biến kiểm tra boss đã chết
    private bool isDialogueActive = false; // Biến kiểm tra nếu UI đang hiển thị

    private void Start()
    {
        itemDropManager = GetComponent<ItemDropManager>();
    }
    private void OnEnable()
    {
        HealthBoss.OnBossDefeated += MarkBossAsDefeated;
    }

    private void OnDisable()
    {
        HealthBoss.OnBossDefeated -= MarkBossAsDefeated;
    }

    private void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(interactionKey))
        {
            if (!isDialogueActive)
            {
                if (!bossDefeated)
                {
                    ShowDialogue("Hãy đánh bại boss trước khi quay lại nói chuyện với tôi!");
                }
                else
                {
                    ShowDialogue("Bạn đã đánh bại boss! Đây là phần thưởng của bạn!");
                    GiveReward();
                }
            }
            else
            {
                HideDialogue(); // Tắt UI khi người chơi nhấn phím lần nữa
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            HideDialogue(); // Tắt UI khi người chơi rời xa
        }
    }

    private void ShowDialogue(string message)
    {
        if (dialogueUI != null && dialogueText != null)
        {
            dialogueText.text = message; // Gán thông báo vào Text
            dialogueUI.SetActive(true); // Hiển thị UI
            isDialogueActive = true;
        }
    }

    private void HideDialogue()
    {
        if (dialogueUI != null)
        {
            dialogueUI.SetActive(false); // Ẩn UI
            isDialogueActive = false;
        }
    }

    private void GiveReward()
    {
        if (itemDropManager != null && dropPosition != null)
        {
            itemDropManager.TryDropLoot(dropPosition.position);
        }
    }

    private void MarkBossAsDefeated()
    {
        bossDefeated = true;
    }
}