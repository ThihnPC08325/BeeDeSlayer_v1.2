using UnityEngine;

public class SmokeEffect : MonoBehaviour
{
    [SerializeField] private float riseSpeed = 1f; // Speed at which the smoke rises
    [SerializeField] private float lifetime = 2f; // Time before the smoke disappears

    private void Start()
    {
        // Destroy the smoke after `lifetime` seconds
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        // Move the smoke upwards over time
        transform.position += Vector3.up * riseSpeed * Time.deltaTime;
    }
}