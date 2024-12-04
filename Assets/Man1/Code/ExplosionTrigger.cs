using UnityEngine;

public class ExplosionTrigger : MonoBehaviour
{
    public GameObject explosionPrefab; // Prefab của hiệu ứng nổ
    public float destroyDelay = 0.5f;  // Thời gian xóa model sau khi nổ
    public int numberOfExplosions = 5; // Số lần tạo hiệu ứng nổ
    public float radius = 10f;         // Bán kính tạo hiệu ứng nổ ngẫu nhiên
    public float cameraShakeRadius = 15f; // Bán kính mà camera sẽ rung
    public float shakeDuration = 0.5f; // Thời gian rung
    public float shakeMagnitude = 0.2f; // Cường độ rung

    private bool hasExploded = false;  // Kiểm tra đã nổ hay chưa
    private Transform playerCamera;    // Tham chiếu đến camera của player

    void Start()
    {
        // Lấy camera của player
        playerCamera = Camera.main.transform;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") && !hasExploded)
        {
            hasExploded = true;

            for (int i = 0; i < numberOfExplosions; i++)
            {
                Vector3 randomPosition = transform.position + new Vector3(
                    Random.Range(-radius, radius),
                    0,
                    Random.Range(-radius, radius)
                );

                GameObject explosion = Instantiate(explosionPrefab, randomPosition, Quaternion.identity);
                Destroy(explosion, destroyDelay);
            }

            // Kiểm tra nếu player nằm trong bán kính rung camera
            if (playerCamera != null && Vector3.Distance(transform.position, playerCamera.position) <= cameraShakeRadius)
            {
                CameraS  shake = playerCamera.GetComponent<CameraS>();
                if (shake != null)
                {
                    StartCoroutine(shake.Shake(shakeDuration, shakeMagnitude));
                }
            }

            // Xóa model sau một thời gian
            Destroy(gameObject, destroyDelay);
        }
    }
}
