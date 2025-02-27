using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedFire : MonoBehaviour
{
    public GameObject firePrefab; // Prefab của cầu lửa
    public Transform firePoint; // Điểm bắn
    public float fireRate = 2f; // Tần suất bắn
    private float fireCooldown; // Thời gian chờ giữa các lần bắn

    private Transform player; // Tham chiếu đến người chơi
    public float attackRange = 10f; // Khoảng cách tấn công

    private void Start()
    {
        // Tìm đối tượng người chơi
        player = GameObject.FindGameObjectWithTag("Player").transform;
        fireCooldown = 0f;
    }

    private void Update()
    {
        fireCooldown -= Time.deltaTime;

        // Kiểm tra khoảng cách đến người chơi
        if (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            // Kiểm tra xem kẻ thù có thể bắn không
            if (fireCooldown <= 0)
            {
                Shoot();
                fireCooldown = fireRate; // Đặt lại thời gian chờ
            }
        }
    }

    private void Shoot()
    {
        // Tạo cầu lửa tại điểm bắn
        GameObject fire = Instantiate(firePrefab, firePoint.position, firePoint.rotation);
        Fire fireScript = fire.GetComponent<Fire>();
        if (fireScript != null)
        {
            fireScript.SetTarget(player.position); // Thiết lập mục tiêu cho cầu lửa
        }
    }
}