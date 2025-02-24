using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTornadoBossMap2 : MonoBehaviour
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private float damagePen = 0f;
    [SerializeField] private float slowPercentage = 0.8f; // Giảm 50% tốc độ
    [SerializeField] private AudioSource tornadoSound; // Thêm AudioSource

    private void Start()
    {
        if (tornadoSound != null)
        {
            tornadoSound.Play(); // Phát âm thanh khi tornado xuất hiện
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameEvents.TriggerPlayerHit(damage, damagePen);
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.ApplySlow(slowPercentage);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.RemoveSlow();
            }
        }
    }

    private void OnDestroy()
    {
        if (tornadoSound != null)
        {
            tornadoSound.Stop(); // Dừng âm thanh khi tornado biến mất
        }
    }
}
