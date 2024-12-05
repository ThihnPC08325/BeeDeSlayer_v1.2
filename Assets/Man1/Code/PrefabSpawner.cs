using UnityEngine;

public class PrefabSpawner : MonoBehaviour
{
    public GameObject prefabToSpawn; // Prefab cần spawn
    public float spawnInterval = 5f; // Khoảng thời gian spawn mỗi lần
    public float spawnHeight = 10f; // Độ cao mà prefab sẽ spawn

    // Giới hạn khu vực spawn (tọa độ bản đồ)
    public float xMin = -10f;
    public float xMax = 10f;
    public float zMin = -10f;
    public float zMax = 10f;

    private void Start()
    {
        // Gọi hàm spawn liên tục
        InvokeRepeating(nameof(SpawnPrefab), 0f, spawnInterval);
    }

    private void SpawnPrefab()
    {
        // Tạo vị trí spawn ngẫu nhiên trong khu vực giới hạn
        Vector3 spawnPosition = new Vector3(
            Random.Range(xMin, xMax), // X trong khoảng giới hạn
            spawnHeight,             // Độ cao cố định
            Random.Range(zMin, zMax) // Z trong khoảng giới hạn
        );

        // Spawn prefab tại vị trí xác định
        Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
    }
}
