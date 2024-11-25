using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightEyeBoss : MonoBehaviour
{
    [SerializeField] private Color lightColor = Color.yellow; // Màu ánh sáng
    [SerializeField] private float lightIntensity = 5f; // Độ sáng của ánh sáng
    [SerializeField] private float lightRange = 10f; // Phạm vi ánh sáng

    private Light objectLight;

    void Start()
    {
        // Thêm một light component nếu chưa có
        objectLight = gameObject.AddComponent<Light>();
        objectLight.type = LightType.Point; // Loại ánh sáng là Point (có thể đổi sang Spot, Directional)
        objectLight.color = lightColor;
        objectLight.intensity = lightIntensity;
        objectLight.range = lightRange;
    }
}
