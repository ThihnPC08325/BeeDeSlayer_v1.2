using Unity.Mathematics;
using UnityEngine;

public readonly struct SimpleMuzzleVelocityCalculator
{
    private readonly float barrelArea;
    private readonly float barrelVolume;

    public SimpleMuzzleVelocityCalculator(float bulletDiameter, float barrelLength)
    {
        float bulletRadius = bulletDiameter * 0.5f;
        barrelArea = math.PI * bulletRadius * bulletRadius;
        barrelVolume = barrelArea * barrelLength;
    }

    public float Calculate(float bulletMass, float averagePressure)
    {
        // Tính công thực hiện bởi khí gas
        float work = averagePressure * barrelVolume;
        // Động năng chuyển thành vận tốc đầu nòng
        return math.sqrt(2f * work / bulletMass);
    }
}

public class CalculateMuzzleVelocity : MonoBehaviour
{
    [Header("Bullet Properties")]
    [SerializeField] private float bulletMass = 0.01f;
    [SerializeField] private float bulletDiameter = 0.00556f;

    [Header("Barrel Properties")]
    [SerializeField] private float barrelLength = 0.5f;
    [SerializeField] private float averagePressure = 15000000f; // Áp suất trung bình

    // Bộ tính toán
    private SimpleMuzzleVelocityCalculator calculator;

    private void Awake()
    {
        calculator = new SimpleMuzzleVelocityCalculator(bulletDiameter, barrelLength);
    }

    public float MuzzleVelocity()
    {
        return calculator.Calculate(bulletMass, averagePressure);
    }
}