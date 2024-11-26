using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance; // Singleton để truy cập dễ dàng từ các script khác

    private Transform camTransform;
    private Vector3 originalPosition;

    [SerializeField] private float shakeDuration = 0.8f; // Thời gian rung
    [SerializeField] private float shakeMagnitude = 2f; // Độ mạnh của rung
    private float currentShakeDuration;

    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        camTransform = Camera.main.transform; // Lấy Transform của camera chính
        originalPosition = camTransform.localPosition;
    }

    private void Update()
    {
        if (currentShakeDuration > 0)
        {
            // Thêm chuyển động ngẫu nhiên vào vị trí camera
            camTransform.localPosition = originalPosition + Random.insideUnitSphere * shakeMagnitude;

            // Giảm thời gian rung
            currentShakeDuration -= Time.deltaTime;

            // Đặt lại vị trí camera sau khi rung xong
            if (currentShakeDuration <= 0)
            {
                camTransform.localPosition = originalPosition;
            }
        }
    }

    public void TriggerShake(float duration, float magnitude)
    {
        currentShakeDuration = duration;
        shakeMagnitude = magnitude;
    }
}
