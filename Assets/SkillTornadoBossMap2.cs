using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTornadoBossMap2 : MonoBehaviour
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private float damagePen = 0f;
    [SerializeField] private float slowPercentage = 0.8f; // Giảm 50% tốc độ

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
}
