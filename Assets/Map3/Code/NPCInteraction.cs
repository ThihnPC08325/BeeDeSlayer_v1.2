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

    private bool _isPlayerNear = false;
    private bool _bossDefeated = false; // Biến kiểm tra boss đã chết
    private bool _isDialogueActive = false; // Biến kiểm tra nếu UI đang hiển thị

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
        if (!_isPlayerNear || !Input.GetKeyDown(interactionKey)) return;
        if (!_isDialogueActive)
        {
            if (!_bossDefeated)
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isPlayerNear = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        _isPlayerNear = false;
        HideDialogue(); // Tắt UI khi người chơi rời xa
    }

    private void ShowDialogue(string message)
    {
        if (!dialogueUI || !dialogueText) return;
        dialogueText.text = message; // Gán thông báo vào Text
        dialogueUI.SetActive(true); // Hiển thị UI
        _isDialogueActive = true;
    }

    private void HideDialogue()
    {
        if (!dialogueUI) return;
        dialogueUI.SetActive(false); // Ẩn UI
        _isDialogueActive = false;
    }

    private void GiveReward()
    {
        if (itemDropManager && dropPosition)
        {
            itemDropManager.TryDropLoot(dropPosition.position);
        }
    }

    private void MarkBossAsDefeated()
    {
        _bossDefeated = true;
    }
}