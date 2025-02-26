using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class meteorite : MonoBehaviour
{
    [Header("Cài đặt vành đai")]
    public GameObject asteroidPrefab; // Prefab của thiên thạch
    public int numberOfAsteroids = 50; // Số lượng thiên thạch
    public float radius = 10f; // Bán kính vành đai
    public float minSpeed = 1f; // Tốc độ xoay tối thiểu
    public float maxSpeed = 3f; // Tốc độ xoay tối đa
    public float thickness = 1f; // Độ dày của vành đai (dao động bán kính)

    private GameObject[] asteroids; // Mảng lưu trữ thiên thạch
    private float[] speeds; // Tốc độ quay của từng thiên thạch
    private float[] angles; // Góc hiện tại của từng thiên thạch

    void Start()
    {
        // Khởi tạo mảng
        asteroids = new GameObject[numberOfAsteroids];
        speeds = new float[numberOfAsteroids];
        angles = new float[numberOfAsteroids];

        // Sinh ra thiên thạch
        for (int i = 0; i < numberOfAsteroids; i++)
        {
            // Tạo thiên thạch từ prefab
            asteroids[i] = Instantiate(asteroidPrefab, Vector3.zero, Quaternion.identity);
            asteroids[i].transform.SetParent(transform); // Gắn vào tâm để dễ quản lý

            // Tính góc ngẫu nhiên ban đầu
            angles[i] = Random.Range(0f, 360f);

            // Tính tốc độ ngẫu nhiên
            speeds[i] = Random.Range(minSpeed, maxSpeed);

            // Đặt kích thước ngẫu nhiên (tùy chọn)
            float scale = Random.Range(0.5f, 1.5f);
            asteroids[i].transform.localScale = Vector3.one * scale;

            // Đặt vị trí ban đầu
            UpdateAsteroidPosition(i);
        }
    }

    void Update()
    {
        // Cập nhật vị trí thiên thạch mỗi frame
        for (int i = 0; i < numberOfAsteroids; i++)
        {
            // Tăng góc dựa trên tốc độ và thời gian
            angles[i] += speeds[i] * Time.deltaTime;

            // Giữ góc trong khoảng 0-360
            if (angles[i] >= 360f) angles[i] -= 360f;

            // Cập nhật vị trí thiên thạch
            UpdateAsteroidPosition(i);
        }
    }

    void UpdateAsteroidPosition(int index)
    {
        // Tính vị trí trên vòng tròn
        float radian = angles[index] * Mathf.Deg2Rad;
        float variedRadius = radius + Random.Range(-thickness, thickness); // Dao động bán kính
        float x = Mathf.Cos(radian) * variedRadius;
        float z = Mathf.Sin(radian) * variedRadius;

        // Đặt vị trí thiên thạch
        asteroids[index].transform.localPosition = new Vector3(x, 0, z);

        // (Tùy chọn) Xoay thiên thạch để trông tự nhiên hơn
        asteroids[index].transform.Rotate(Vector3.up, speeds[index] * Time.deltaTime * 10f);
    }

    // (Tùy chọn) Xem trước trong Editor
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
