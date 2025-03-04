using UnityEngine;

public class BulletDirectionCalculator : MonoBehaviour
{
    private const float TwoPI = 2f * Mathf.PI;
    private static readonly Vector3 DefaultDirection = Vector3.forward;

    private static float[] _precomputedSin;
    private static float[] _precomputedCos;
    private const int PrecomputeSteps = 180;
    private static bool _isInitialized = false;
    private static Transform _cachedBulletSpawn;
    private static Vector3 _lastKnownDirection;

    // Cache Quaternion để tái sử dụng
    private static Quaternion[] _cachedRotations;
    private const int RotationCacheSize = 360;

    private void Awake()
    {
        InitializeIfNeeded();
    }

    private static void InitializeIfNeeded()
    {
        if (_isInitialized) return;
        // Khởi tạo các mảng
        _precomputedSin = new float[PrecomputeSteps];
        _precomputedCos = new float[PrecomputeSteps];
        _cachedRotations = new Quaternion[RotationCacheSize];

        // Precompute các giá trị
        for (int i = 0; i < PrecomputeSteps; i++)
        {
            float angle = (TwoPI * i) / PrecomputeSteps;
            _precomputedSin[i] = Mathf.Sin(angle);
            _precomputedCos[i] = Mathf.Cos(angle);
        }

        for (int i = 0; i < RotationCacheSize; i++)
        {
            _cachedRotations[i] = Quaternion.Euler(0, i, 0);
        }

        _isInitialized = true;
    }

    public static Vector3 CalculateDirection(Transform bulletSpawn)
    {
        InitializeIfNeeded();

        if (!bulletSpawn)
        {
            Debug.LogError("BulletSpawn transform is null!");
            return _lastKnownDirection != Vector3.zero ? _lastKnownDirection : DefaultDirection;
        }

        // Cache bulletSpawn reference
        if (_cachedBulletSpawn && _cachedBulletSpawn != bulletSpawn)
        {
            _cachedBulletSpawn = bulletSpawn;
        }

        // Tính toán direction với độ ổn định cao hơn
        Vector3 baseDirection = bulletSpawn.TransformDirection(Vector3.forward);
        Quaternion initialRotation = Quaternion.LookRotation(baseDirection);

        int angleIndex = Random.Range(0, PrecomputeSteps);
        float u1 = Mathf.Clamp01(1f - Random.value); // Đảm bảo giá trị hợp lệ
        float sqrtLog = Mathf.Sqrt(-2f * Mathf.Log(u1));

        float spreadX = sqrtLog * _precomputedCos[angleIndex];
        float spreadY = sqrtLog * _precomputedSin[angleIndex];

        int rotationIndex = Mathf.Abs(Mathf.RoundToInt(spreadY)) % RotationCacheSize;

        Vector3 finalDirection = initialRotation * _cachedRotations[rotationIndex] * Vector3.forward;
        _lastKnownDirection = finalDirection;

        return finalDirection;
    }

    // Thêm method để reset calculator khi cần
    public void Reset()
    {
        _isInitialized = false;
        _cachedBulletSpawn = null;
        _lastKnownDirection = Vector3.zero;
    }
}