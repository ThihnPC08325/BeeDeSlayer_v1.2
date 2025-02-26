using UnityEngine;

public class ExplosionTrigger : MonoBehaviour
{
    [SerializeField] private GameObject explosionPrefab; // Prefab của hiệu ứng nổ
    [SerializeField] private float destroyDelay = 0.5f;  // Thời gian xóa model sau khi nổ
    [SerializeField] private int numberOfExplosions = 5; // Số lần tạo hiệu ứng nổ
    [SerializeField] private float radius = 10f;         // Bán kính tạo hiệu ứng nổ ngẫu nhiên
    [SerializeField] private float cameraShakeRadius = 15f; // Bán kính mà camera sẽ rung
    [SerializeField] private float shakeDuration = 0.5f; // Thời gian rung
    [SerializeField] private float shakeMagnitude = 0.2f; // Cường độ rung

    private bool _hasExploded = false;  // Kiểm tra đã nổ hay chưa
    private Transform _playerCamera;    // Tham chiếu đến camera của player

    void Start()
    {
        // Lấy camera của player
        _playerCamera = Camera.main.transform;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Ground") || _hasExploded) return;
        _hasExploded = true;

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
        if (_playerCamera != null && Vector3.Distance(transform.position, _playerCamera.position) <= cameraShakeRadius)
        {
            CameraS  shake = _playerCamera.GetComponent<CameraS>();
            if (shake != null)
            {
                StartCoroutine(shake.Shake(shakeDuration, shakeMagnitude));
            }
        }

        // Xóa model sau một thời gian
        Destroy(gameObject, destroyDelay);
    }
}
