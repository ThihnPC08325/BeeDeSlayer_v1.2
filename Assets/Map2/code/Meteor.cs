using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor : MonoBehaviour
{
    [SerializeField] private float explosionDamage = 50f;  // Sát thương khi rơi xuống
    [SerializeField] private float dotDamage = 5f;  // Sát thương theo thời gian
    [SerializeField] private int dotTicks = 3;  // Số lần sát thương DOT
    [SerializeField] private float dotInterval = 1f;  // Thời gian giữa các lần DOT

    [SerializeField] private GameObject explosionEffect;  // Hiệu ứng nổ

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                Debug.Log($"🔥 Meteor trúng Player! Gây {explosionDamage} sát thương + {dotDamage} DOT x {dotTicks}");

                // Gây sát thương ngay lập tức
                playerHealth.TakeMeteorDamage(explosionDamage, dotDamage, dotTicks, dotInterval);

                // Hiệu ứng nổ
                if (explosionEffect != null)
                {
                    Instantiate(explosionEffect, transform.position, Quaternion.identity);
                }

                // Hủy meteor sau khi va chạm
                Destroy(gameObject);
            }
        }
    }

    public void Activate(float meteorDamage, float dotDmg, int ticks, float interval)
    {
        explosionDamage = meteorDamage;
        dotDamage = dotDmg;
        dotTicks = ticks;
        dotInterval = interval;
    }
}
