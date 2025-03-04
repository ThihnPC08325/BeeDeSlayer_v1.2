using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NPCInteraction : MonoBehaviour
{
    [SerializeField] private KeyCode interactionKey = KeyCode.F;
    [SerializeField] private GameObject dialogueUI;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private ItemDropManager itemDropManager;
    [SerializeField] private Transform dropPosition;
    [SerializeField] private Transform teleportDestination; // Điểm đến dịch chuyển
    [SerializeField] private ParticleSystem teleportEffect; // Hiệu ứng dịch chuyển

    private bool _isPlayerNear = false;
    private bool _bossDefeated = false;
    private bool _isDialogueActive = false;

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
                ShowDialogue("Hãy đánh bại boss và bạn sẽ gặp lại tôi!");
            }
            else
            {
                ShowDialogue("Bạn đã làm được rồi! Đây là phần thưởng của bạn, hãy đi tới cánh cổng kia để đến thử thách cuối cùng!");
                GiveReward();
            }
        }
        else
        {
            HideDialogue();
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
        HideDialogue();
    }

    private void ShowDialogue(string message)
    {
        if (!dialogueUI || !dialogueText) return;
        dialogueText.text = message;
        dialogueUI.SetActive(true);
        _isDialogueActive = true;
    }

    private void HideDialogue()
    {
        if (!dialogueUI) return;
        dialogueUI.SetActive(false);
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
        StartCoroutine(TeleportNpcWithDelay());
    }

    private IEnumerator TeleportNpcWithDelay()
    {
        yield return new WaitForSeconds(0.5f); // Delay để tránh lỗi khi Boss chưa hoàn toàn bị hủy

        if (teleportEffect)
        {
            Instantiate(teleportEffect, transform.position, Quaternion.identity);
        }

        if (!teleportDestination) yield break;
        transform.position = teleportDestination.position;

        if (teleportEffect)
        {
            Instantiate(teleportEffect, transform.position, Quaternion.identity);
        }
    }
}
