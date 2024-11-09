using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowZone : MonoBehaviour
{
    [SerializeField] float slowAmount = 0.5f; // Mức độ làm chậm (0.5 = giảm 50% tốc độ)
    [SerializeField] float duration = 5f; // Thời gian tồn tại của vùng
    [SerializeField] float radius = 5f; // Bán kính vùng làm chậm

    private void Start()
    {
        // Set kích thước vùng
        transform.localScale = Vector3.one * (radius * 2);

        Destroy(gameObject, duration);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Lấy script điều khiển người chơi
            PlayerController playerMovement = other.GetComponent<PlayerController>();
            if (playerMovement != null)
            {
                playerMovement.ApplySpeedModifier(slowAmount);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController playerMovement = other.GetComponent<PlayerController>();
            if (playerMovement != null)
            {
                playerMovement.RemoveSpeedModifier();
            }
        }
    }
}
