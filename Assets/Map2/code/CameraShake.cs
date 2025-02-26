using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance; // Singleton để truy cập dễ dàng từ các script khác

    private Transform _camTransform;
    private Vector3 _originalPosition;

    [SerializeField] private float shakeDuration = 0.8f; // Thời gian rung
    [SerializeField] private float shakeMagnitude = 2f; // Độ mạnh của rung
    private float _currentShakeDuration;

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

        _camTransform = Camera.main.transform; // Lấy Transform của camera chính
        _originalPosition = _camTransform.localPosition;
    }

    private void Update()
    {
        if (!(_currentShakeDuration > 0)) return;
        // Thêm chuyển động ngẫu nhiên vào vị trí camera
        _camTransform.localPosition = _originalPosition + Random.insideUnitSphere * shakeMagnitude;

        // Giảm thời gian rung
        _currentShakeDuration -= Time.deltaTime;

        // Đặt lại vị trí camera sau khi rung xong
        if (_currentShakeDuration <= 0)
        {
            _camTransform.localPosition = _originalPosition;
        }
    }

    public void TriggerShake(float duration, float magnitude)
    {
        _currentShakeDuration = duration;
        shakeMagnitude = magnitude;
    }
}
