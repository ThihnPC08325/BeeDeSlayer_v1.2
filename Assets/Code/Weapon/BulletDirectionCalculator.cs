using UnityEngine;

public class BulletDirectionCalculator : MonoBehaviour
{
    private const float TWO_PI = 2f * Mathf.PI;
    private static Vector3 DEFAULT_DIRECTION = Vector3.forward;

    private static float[] precomputedSin;
    private static float[] precomputedCos;
    private const int PRECOMPUTE_STEPS = 180;
    private static bool isInitialized = false;
    private static Transform cachedBulletSpawn;
    private static Vector3 lastKnownDirection;

    // Cache Quaternion để tái sử dụng
    private static Quaternion[] cachedRotations;
    private const int ROTATION_CACHE_SIZE = 360;

    private void Awake()
    {
        InitializeIfNeeded();
    }

    private static void InitializeIfNeeded()
    {
        if (!isInitialized)
        {
            // Khởi tạo các mảng
            precomputedSin = new float[PRECOMPUTE_STEPS];
            precomputedCos = new float[PRECOMPUTE_STEPS];
            cachedRotations = new Quaternion[ROTATION_CACHE_SIZE];

            // Precompute các giá trị
            for (int i = 0; i < PRECOMPUTE_STEPS; i++)
            {
                float angle = (TWO_PI * i) / PRECOMPUTE_STEPS;
                precomputedSin[i] = Mathf.Sin(angle);
                precomputedCos[i] = Mathf.Cos(angle);
            }

            for (int i = 0; i < ROTATION_CACHE_SIZE; i++)
            {
                cachedRotations[i] = Quaternion.Euler(0, i, 0);
            }

            isInitialized = true;
        }
    }

    public static Vector3 CalculateDirection(Transform bulletSpawn)
    {
        InitializeIfNeeded();

        if (bulletSpawn == null)
        {
            Debug.LogError("BulletSpawn transform is null!");
            return lastKnownDirection != Vector3.zero ? lastKnownDirection : DEFAULT_DIRECTION;
        }

        // Cache bulletSpawn reference
        if (cachedBulletSpawn != bulletSpawn)
        {
            cachedBulletSpawn = bulletSpawn;
        }

        // Tính toán direction với độ ổn định cao hơn
        Vector3 baseDirection = bulletSpawn.TransformDirection(Vector3.forward);
        Quaternion initialRotation = Quaternion.LookRotation(baseDirection);

        int angleIndex = Random.Range(0, PRECOMPUTE_STEPS);
        float u1 = Mathf.Clamp01(1f - Random.value); // Đảm bảo giá trị hợp lệ
        float sqrtLog = Mathf.Sqrt(-2f * Mathf.Log(u1));

        float spreadX = sqrtLog * precomputedCos[angleIndex];
        float spreadY = sqrtLog * precomputedSin[angleIndex];

        int rotationIndex = Mathf.Abs(Mathf.RoundToInt(spreadY)) % ROTATION_CACHE_SIZE;

        Vector3 finalDirection = initialRotation * cachedRotations[rotationIndex] * Vector3.forward;
        lastKnownDirection = finalDirection;

        return finalDirection;
    }

    // Thêm method để reset calculator khi cần
    public static void Reset()
    {
        isInitialized = false;
        cachedBulletSpawn = null;
        lastKnownDirection = Vector3.zero;
    }
}