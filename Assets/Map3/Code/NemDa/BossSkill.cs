﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSkill : MonoBehaviour
{
    [SerializeField] public List<GameObject> rockPrefabs; // Danh sách prefab của các viên đá
    [SerializeField] public Transform throwPoint; // Vị trí xuất phát của viên đá
    [SerializeField] private float throwForce = 100f; // Lực ném đá
    [SerializeField] private float throwInterval = 5f; // Thời gian giữa các lần ném
    [SerializeField] private float delayBeforeThrow = 7f; // Thời gian trì hoãn trước khi bắt đầu ném đá
    private Transform player;

    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
            StartCoroutine(WaitAndThrowRock());
        }
    }

    // Coroutine trì hoãn việc ném đá
    IEnumerator WaitAndThrowRock()
    {
        yield return new WaitForSeconds(delayBeforeThrow);
        StartCoroutine(ThrowRockRoutine());
    }

    IEnumerator ThrowRockRoutine()
    {
        while (true)
        {
            ThrowRock();
            yield return new WaitForSeconds(throwInterval);
        }
    }

    void ThrowRock()
    {
        if (rockPrefabs.Count > 0 && player != null && throwPoint != null)
        {
            // Lấy ngẫu nhiên một prefab từ danh sách
            GameObject selectedRockPrefab = rockPrefabs[Random.Range(0, rockPrefabs.Count)];

            // Tạo viên đá
            GameObject rock = Instantiate(selectedRockPrefab, throwPoint.position, throwPoint.rotation);

            // Gán mục tiêu cho viên đá nếu có script RockBehavior
            RockBehavior rockBehavior = rock.GetComponent<RockBehavior>();
            if (rockBehavior != null)
            {
                rockBehavior.target = player;
            }
        }
    }
}