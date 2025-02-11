using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawLandmine : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject objectToSpawn;     // Prefab của GameObject cần spawn
    [SerializeField] private float spawnRadius = 10f;      // Bán kính phạm vi xung quanh điểm spawn
    [SerializeField] private float spawnHeight = 0f;       // Chiều cao, bạn có thể điều chỉnh nếu cần
    [SerializeField] private int maxLandmines = 5;         // Số lượng landmines tối đa trên bản đồ

    private List<GameObject> landmines = new List<GameObject>(); // List để theo dõi các landmines đã spawn

    void Start()
    {
        StartCoroutine(SpawRedZone());
    }

    private IEnumerator SpawRedZone()
    {
        while (true)
        {
            // Kiểm tra xem có ít hơn 5 landmines trên bản đồ không
            if (landmines.Count < maxLandmines)
            {
                SpawnObjectsInRadius();
            }
            // Chờ 5 giây trước khi kiểm tra lại
            yield return new WaitForSeconds(5f);
        }
    }

    void SpawnObjectsInRadius()
    {
        // Tạo vị trí ngẫu nhiên trong phạm vi xung quanh vị trí của object này
        Vector3 randomPosition = transform.position + Random.insideUnitSphere * spawnRadius;

        // Nếu bạn muốn spawn ở mặt đất hoặc điều chỉnh chiều cao, hãy set y
        randomPosition.y = spawnHeight;

        // Spawn GameObject tại vị trí ngẫu nhiên
        GameObject spawnedObject = Instantiate(objectToSpawn, randomPosition, Quaternion.identity);

        // Thêm object vào danh sách landmines
        landmines.Add(spawnedObject);

        // Đảm bảo object có tag "Landmine"
        spawnedObject.tag = "Landmine";

        // Gắn callback khi object bị phá hủy để loại bỏ khỏi danh sách
        Destroy(spawnedObject, 10f); // Giả sử mỗi landmine bị phá hủy sau 10 giây, bạn có thể thay đổi thời gian này hoặc gọi Destroy khi nổ

        // Xử lý khi đối tượng bị phá hủy
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}
