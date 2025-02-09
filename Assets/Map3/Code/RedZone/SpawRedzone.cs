using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawRedzone : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject objectToSpawn;     // Prefab của GameObject cần spawn
    public float spawnRadius = 10f;      // Bán kính phạm vi xung quanh điểm spawn
    public float spawnHeight = 0f;       // Chiều cao, bạn có thể điều chỉnh nếu cần

    void Start()
    {
        StartCoroutine(SpawRedZone());
    }

    private IEnumerator SpawRedZone()
    {
        while (true)
        {
            // Gọi hàm spawn object
            SpawnObjectsInRadius();
            // Chờ 5 giây
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
        Instantiate(objectToSpawn, randomPosition, Quaternion.identity);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}
