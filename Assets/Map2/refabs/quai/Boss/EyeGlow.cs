using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeGlow : MonoBehaviour
{
    [SerializeField] private Color glowColor = Color.red; // Màu phát sáng
    [SerializeField] private float glowIntensity = 5f; // Độ sáng

    private Renderer objectRenderer;
    private Material objectMaterial;

    void Start()
    {
        // Lấy Renderer và Material của đối tượng
        objectRenderer = GetComponent<Renderer>();
        objectMaterial = objectRenderer.material;

        // Thiết lập ánh sáng phát sáng (Emission)
        if (objectMaterial != null)
        {
            objectMaterial.SetColor("_EmissionColor", glowColor * glowIntensity); // Màu và độ sáng
            // Kích hoạt hiệu ứng phát sáng bằng cách bật Global Illumination
            DynamicGI.SetEmissive(objectRenderer, glowColor * glowIntensity);
        }
    }
}
