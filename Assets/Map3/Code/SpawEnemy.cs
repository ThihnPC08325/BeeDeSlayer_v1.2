using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawEnemy : MonoBehaviour
{
    [Header("Skill Settings")]
    [SerializeField] private GameObject[] spawnPrefabs; // Mảng các prefab sẽ được tạo ra
    [SerializeField] private int[] spawnCounts; // Mảng số lượng spawn tương ứng với từng prefab
    [SerializeField] private Transform[] spawnPositions; // Mảng các Transform chứa các vị trí tạo prefab
    [SerializeField] private int maxPrefabs = 20; // Giới hạn số lượng prefab tối đa có thể tạo ra
    [SerializeField] private float skillCooldown = 10f; // Thời gian hồi chiêu

    [Header("Boss Health Reference")]
    [SerializeField] private HealthBoss bossHealth; // Tham chiếu đến mã máu của boss

    private int _totalPrefabsCreated = 0; // Tổng số prefabs đã tạo ra
    private int _spawnIndex = 0; // Chỉ số theo dõi vị trí tiếp theo trong spawnPositions

    private void Start()
    {

        if (spawnPrefabs.Length == 0 || spawnPositions.Length == 0 || spawnCounts.Length != spawnPrefabs.Length)
        {
            enabled = false;
            return;
        }

        // Bắt đầu vòng lặp kiểm tra chiêu mỗi 10 giây
        StartCoroutine(SkillCheckRoutine());
    }

    private IEnumerator SkillCheckRoutine()
    {
        while (true)
        {
            // Kiểm tra điều kiện để sử dụng chiêu
            if (bossHealth && bossHealth.currentHealth > bossHealth.maxHealth / 2 && _totalPrefabsCreated < maxPrefabs)
            {
                Debug.Log("Using skill!");
                SpawnPrefabsByCounts(); // Tạo số lượng prefab tùy chỉnh
            }

            yield return new WaitForSeconds(skillCooldown); // Mỗi 10 giây sẽ kiểm tra lại
        }
    }

    private void SpawnPrefabsByCounts()
    {
        for (int i = 0; i < spawnPrefabs.Length; i++)
        {
            // Lấy số lượng spawn tương ứng với prefab
            int count = spawnCounts[i];

            // Kiểm tra tổng số prefabs đã tạo ra
            if (_totalPrefabsCreated + count > maxPrefabs)
            {
                count = maxPrefabs - _totalPrefabsCreated; // Giới hạn số lượng spawn không vượt quá maxPrefabs
            }

            // Tạo từng prefab
            for (int j = 0; j < count; j++)
            {
                if (_spawnIndex >= spawnPositions.Length)
                {
                    _spawnIndex = 0; // Nếu hết vị trí, vòng lại từ đầu
                }

                // Lấy vị trí spawn từ mảng spawnPositions
                Transform spawnPosition = spawnPositions[_spawnIndex];

                // Tạo prefab tại vị trí spawn
                Instantiate(spawnPrefabs[i], spawnPosition.position, spawnPosition.rotation);

                _spawnIndex++; // Cập nhật spawnIndex
                _totalPrefabsCreated++; // Cập nhật tổng số prefabs đã tạo

                // Dừng nếu đạt giới hạn
                if (_totalPrefabsCreated >= maxPrefabs)
                {
                    return;
                }
            }
        }
    }
}
