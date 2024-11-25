using UnityEngine;

public class BulletDirectionCalculator : MonoBehaviour
{
    private const float TWO_PI = 2f * Mathf.PI;
    private static readonly Vector3 DEFAULT_DIRECTION = Vector3.forward;

    private static readonly float[] precomputedSin;
    private static readonly float[] precomputedCos;
    private const int PRECOMPUTE_STEPS = 36;

    // Cache Quaternion để tái sử dụng
    private static readonly Quaternion[] cachedRotations;
    private const int ROTATION_CACHE_SIZE = 360;

    static BulletDirectionCalculator()
    {
        precomputedSin = new float[PRECOMPUTE_STEPS];
        precomputedCos = new float[PRECOMPUTE_STEPS];
        cachedRotations = new Quaternion[ROTATION_CACHE_SIZE];

        for (int i = 0; i < PRECOMPUTE_STEPS; i++)
        {
            float angle = (TWO_PI * i) / PRECOMPUTE_STEPS;
            precomputedSin[i] = Mathf.Sin(angle);
            precomputedCos[i] = Mathf.Cos(angle);
        }

        // Pre-compute rotations
        for (int i = 0; i < ROTATION_CACHE_SIZE; i++)
        {
            cachedRotations[i] = Quaternion.Euler(0, i, 0);
        }
    }

    public static Vector3 CalculateDirection(Transform bulletSpawn, float spreadIntensity)
    {
        if (bulletSpawn == null)
        {
            Debug.LogError("BulletSpawn transform is null!");
            return DEFAULT_DIRECTION;
        }

        Vector3 baseDirection = bulletSpawn.forward;

        // Sử dụng UnityEngine.Random
        int angleIndex = Random.Range(0, PRECOMPUTE_STEPS);
        float u1 = 1f - Random.value;

        float sqrtLog = Mathf.Sqrt(-2f * Mathf.Log(u1));

        float spreadX = sqrtLog * precomputedCos[angleIndex] * spreadIntensity;
        float spreadY = sqrtLog * precomputedSin[angleIndex] * spreadIntensity;

        // Sử dụng cached rotation
        int rotationIndex = Mathf.RoundToInt(spreadY) % ROTATION_CACHE_SIZE;
        if (rotationIndex < 0) rotationIndex += ROTATION_CACHE_SIZE;

        return cachedRotations[rotationIndex] * baseDirection;
    }
}