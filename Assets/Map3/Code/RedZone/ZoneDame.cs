﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneDame : MonoBehaviour
{
    [SerializeField] private float initialRadius = 0f;    // Bán kính ban đầu (khi chưa kích hoạt)
    [SerializeField] private int maxRadius = 65;        // Bán kính tối đa
    [SerializeField] private float expansionTime = 2f;     // Thời gian để tăng kích thước từ ban đầu đến tối đa
    [SerializeField] private float redZoneDamage = 50f;    // Sát thương của vùng đỏ
    [SerializeField] private SphereCollider zoneCollider;
    private PlayerHealth _playerHealth;

    private void Start()
    {
        zoneCollider.radius = initialRadius; // Bắt đầu với bán kính ban đầu (0 hoặc giá trị nhỏ)
        StartCoroutine(ExpandCollider());
        _playerHealth = FindObjectOfType<PlayerHealth>(); // Tìm script PlayerHealth trong scene
    }

    private void OnTriggerEnter(Collider other)
    {
        TakeZoneDamage(); // Khi player va chạm với vùng đỏ, gọi hàm TakeZonedame
    }


    private void TakeZoneDamage()
    {
        // Lấy tất cả các collider trong phạm vi vùng đỏ
        Collider[] playerColliders = Physics.OverlapSphere(transform.position, zoneCollider.radius);
        foreach (Collider nearby in playerColliders)
        {
            // Nếu đối tượng có tag "Player", áp dụng sát thương
            if (!nearby.CompareTag("Player")) continue;
            // Kiểm tra và lấy script PlayerHealth của player
            if (_playerHealth == null) continue;
            _playerHealth.TakeDamage(redZoneDamage, 0); // Truyền giá trị sát thương và có thể thay đổi giá trị penetration nếu cần
            Destroy(gameObject); // Xóa vùng đỏ sau khi
        }
    }

    private IEnumerator ExpandCollider()
    {
        float elapsedTime = 0f;

        // Tăng kích thước của collider từ ban đầu đến bán kính tối đa
        while (elapsedTime < expansionTime)
        {
            zoneCollider.radius = Mathf.Lerp(initialRadius, maxRadius, elapsedTime / expansionTime);
            elapsedTime += Time.deltaTime;
            yield return null; // Đợi cho đến frame tiếp theo
        }

        zoneCollider.radius = maxRadius; // Đảm bảo collider đạt đến bán kính tối đa sau khi hoàn thành
    }
}
