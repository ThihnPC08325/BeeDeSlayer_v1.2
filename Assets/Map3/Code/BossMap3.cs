using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMap3 : MonoBehaviour
{
    [Header("Skill Settings")]
    [SerializeField] private GameObject[] spawnPrefabs; // Mảng các prefab sẽ được tạo ra
    [SerializeField] private Transform[] spawnPositions; // Mảng các Transform chứa các vị trí tạo prefab
    [SerializeField] private int prefabCount = 5; // Số lượng prefab tạo ra mỗi lần (có thể thay đổi trong Inspector)
    [SerializeField] private int maxPrefabs = 20; // Giới hạn số lượng prefab tối đa có thể tạo ra
    [SerializeField] private float skillCooldown = 10f; // Thời gian hồi chiêu

    [Header("Boss Health Reference")]
    [SerializeField] private BossMap3Health bossHealth; // Tham chiếu đến mã máu của boss

    private int totalPrefabsCreated = 0; // Tổng số prefabs đã tạo ra
    private int spawnIndex = 0; // Chỉ số theo dõi vị trí tiếp theo trong spawnPositions

    private void Start()
    {
        if (bossHealth == null)
        {
            bossHealth = GetComponent<BossMap3Health>();
        }

        if (spawnPrefabs.Length == 0 || spawnPositions.Length == 0)
        {
            Debug.LogError("Spawn Prefabs or Spawn Positions are not assigned!");
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
            if (bossHealth != null && bossHealth.CurrentHealth > bossHealth.MaxHealth / 2 && totalPrefabsCreated < maxPrefabs)
            {
                Debug.Log("Using skill!");
                SpawnPrefabsAround(prefabCount); // Tạo ra số lượng prefab tùy chỉnh
            }

            yield return new WaitForSeconds(skillCooldown); // Mỗi 10 giây sẽ kiểm tra lại
        }
    }

    private void SpawnPrefabsAround(int count)
    {
        // Kiểm tra tổng số prefabs đã tạo ra
        if (totalPrefabsCreated + count > maxPrefabs)
        {
            count = maxPrefabs - totalPrefabsCreated; // Giới hạn số prefab tạo thêm không vượt quá maxPrefabs
        }

        for (int i = 0; i < count; i++)
        {
            if (spawnIndex >= spawnPositions.Length)
            {
                spawnIndex = 0; // Nếu hết vị trí, vòng lại từ đầu
            }

            // Chọn prefab ngẫu nhiên từ mảng spawnPrefabs
            GameObject randomPrefab = spawnPrefabs[Random.Range(0, spawnPrefabs.Length)];

            // Lấy vị trí spawn từ mảng spawnPositions
            Transform spawnPosition = spawnPositions[spawnIndex];

            // Tạo prefab tại vị trí spawn
            Instantiate(randomPrefab, spawnPosition.position, spawnPosition.rotation);

            spawnIndex++; // Cập nhật spawnIndex
            totalPrefabsCreated++; // Cập nhật tổng số prefabs đã tạo
        }
    }
}
