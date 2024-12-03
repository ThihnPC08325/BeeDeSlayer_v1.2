using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMap3 : MonoBehaviour
{
    [Header("Skill Settings")]
    [SerializeField] private GameObject spawnPrefab; // Prefab sẽ được tạo ra
    [SerializeField] private Transform[] spawnPositions; // Mảng các Transform chứa các vị trí tạo prefab
    [SerializeField] private int prefabCount = 5; // Số lượng prefab tạo ra mỗi lần (có thể thay đổi trong Inspector)
    [SerializeField] private int maxPrefabs = 20; // Giới hạn số lượng prefab tối đa có thể tạo ra
    [SerializeField] private float skillCooldown = 10f; // Thời gian hồi chiêu

    [Header("Boss Health Reference")]
    [SerializeField] private BossMap3Health bossHealth; // Tham chiếu đến mã máu của boss

    private int totalPrefabsCreated = 0; // Tổng số prefabs đã tạo ra
    private int spawnIndex = 0; // Chỉ số theo dõi vị trí tiếp theo trong spawnPositions
    private bool isCooldown = false; // Kiểm tra trạng thái hồi chiêu

    private void Start()
    {
        if (bossHealth == null)
        {
            bossHealth = GetComponent<BossMap3Health>();
        }

        if (spawnPrefab == null || spawnPositions.Length == 0)
        {
            Debug.LogError("Spawn Prefab or Spawn Positions are not assigned!");
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
            if (!isCooldown && bossHealth.CurrentHealth > bossHealth.MaxHealth / 2 && totalPrefabsCreated < maxPrefabs)
            {
                Debug.Log("Using skill!");
                SpawnPrefabsAround(prefabCount); // Tạo ra số lượng prefab tùy chỉnh
            }

            yield return new WaitForSeconds(skillCooldown); // Mỗi 10 giây sẽ tạo thêm prefabs
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
            // Kiểm tra xem có đủ vị trí không
            if (spawnIndex < spawnPositions.Length)
            {
                // Lấy vị trí từ spawnPositions
                Transform spawnPosition = spawnPositions[spawnIndex];
                Instantiate(spawnPrefab, spawnPosition.position, spawnPosition.rotation);

                // Cập nhật spawnIndex để tạo prefab tại vị trí tiếp theo
                spawnIndex++;

                totalPrefabsCreated++; // Cập nhật tổng số prefabs đã tạo
            }
            else
            {
                // Nếu hết vị trí trong mảng, vòng lại từ đầu
                spawnIndex = 0;
            }
        }
    }
}
