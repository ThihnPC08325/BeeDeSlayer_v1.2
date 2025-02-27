using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;

public class TrapSpawner : MonoBehaviour
{
    [SerializeField] private GameObject trapPrefab; // Prefab bẫy (hiệu ứng rơi + Collider)
    [SerializeField] private float spawnInterval = 5f; // Thời gian bẫy xuất hiện lại
    [SerializeField] Vector3 spawnAreaSize = new Vector3(10, 0, 10); // Khu vực spawn bẫy
    [SerializeField] private int damage = 10; // Sát thương khi va chạm

    private void Start()
    {
        StartCoroutine(SpawnTrap());
    }

    IEnumerator SpawnTrap()
    {
        while (true) // Lặp vô hạn
        {
            Vector3 spawnPos = GetRandomSpawnPosition(); // Lấy vị trí ngẫu nhiên
            GameObject trap = Instantiate(trapPrefab, spawnPos, Quaternion.identity); // Tạo bẫy

            Trap trapScript = trap.AddComponent<Trap>(); // Thêm script xử lý va chạm
            trapScript.damagePerSecond = damage; // Truyền sát thương cho bẫy

            yield return new WaitForSeconds(spawnInterval); // Chờ 5 giây

            Destroy(trap); // Xóa bẫy sau 5 giây
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        float randomX = Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2);
        float randomZ = Random.Range(-spawnAreaSize.z / 2, spawnAreaSize.z / 2);
        return new Vector3(randomX, 10f, randomZ); // Xuất hiện trên cao rồi rơi xuống
    }
}
