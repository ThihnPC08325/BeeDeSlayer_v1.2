using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTornadoBossMap2 : MonoBehaviour
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private float damagePen = 0f;
    [SerializeField] private float slowPercentage = 0.8f; // Giảm 80% tốc độ
    [SerializeField] private float damageInterval = 1f; // Sát thương mỗi giây
    [SerializeField] private AudioClip tornadoSound; // Âm thanh lốc xoáy

    private AudioSource audioSource;
    private float nextDamageTime = 0f;

    private void Start()
    {
        // Thêm âm thanh lốc xoáy
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = tornadoSound;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.Play();

        // Hủy object sau 20 giây
        Destroy(gameObject, 20f);
    }

    private void OnDestroy()
    {
        if (audioSource != null)
        {
            audioSource.Stop(); // Dừng âm thanh khi bị hủy
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                // Luôn giảm tốc khi ở trong lốc xoáy
                player.ApplySlow(slowPercentage);

                // Gây sát thương mỗi giây
                if (Time.time >= nextDamageTime)
                {
                    GameEvents.TriggerPlayerHit(damage, damagePen);
                    nextDamageTime = Time.time + damageInterval;
                }
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
                player.RemoveSlow(); // Hết làm chậm khi rời khỏi
            }
        }
    }
}