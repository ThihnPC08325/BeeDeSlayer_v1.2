using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BrokenFlyDebuff : MonoBehaviour
{
    [SerializeField] private Material debuffMaterial;
    [SerializeField] private float fadeInDuration = 1.5f;  // Time to reach max intensity
    [SerializeField] private float fadeOutDuration = 3f;   // Time to fade out after duration
    private RawImage debuffOverlay;

    // Max values from the image
    private const float MAX_VIGNETTE_INTENSITY = 1.5f;
    private const float MAX_VIGNETTE_POWER = 3f;
    private static readonly Color MAX_VIGNETTE_COLOR = Color.green;  // Set to HDR Green
    private const float MAX_VORONOI_SPEED = 3f;
    private const float MAX_VORONOI_INTENSITY = 1f;
    private const float MAX_OUTLINE_THICKNESS = 3f;
    private const float MAX_VORONOI_POWER = 5f;
    private const float MAX_VORONOI_DENSITY = 30f;

    void Awake()
    {
        debuffOverlay = GetComponent<RawImage>();
        if (debuffOverlay != null)
        {
            debuffMaterial = Instantiate(debuffOverlay.material); // Clone material to prevent overriding the original
            debuffOverlay.material = debuffMaterial;
        }

        // Set all properties to 0 at the start
        ResetDebuffProperties();
    }

    void ResetDebuffProperties()
    {
        debuffMaterial.SetFloat("_VignetteIntensity", 0f);
        debuffMaterial.SetFloat("_VignettePower", 0f);
        debuffMaterial.SetColor("_VignetteColor", Color.black); // Start with black to avoid flash
        debuffMaterial.SetFloat("_VoronoiIntensity", 0f);
        debuffMaterial.SetFloat("_OutlineThickness", 0f);
        debuffMaterial.SetFloat("_VoronoiPower", 0f);
    }

    public void ApplyDebuff(float duration)
    {
        Debug.Log("Debuff started");
        StartCoroutine(DebuffEffect(duration));
    }

    private IEnumerator DebuffEffect(float duration)
    {
        float elapsedTime = 0f;

        // Fade In
        while (elapsedTime < fadeInDuration)
        {
            float t = elapsedTime / fadeInDuration;
            debuffMaterial.SetFloat("_VignetteIntensity", Mathf.Lerp(0, MAX_VIGNETTE_INTENSITY, t));
            debuffMaterial.SetFloat("_VignettePower", Mathf.Lerp(0, MAX_VIGNETTE_POWER, t));
            debuffMaterial.SetColor("_VignetteColor", Color.Lerp(Color.black, MAX_VIGNETTE_COLOR, t));
            debuffMaterial.SetFloat("_VoronoiIntensity", Mathf.Lerp(0, MAX_VORONOI_INTENSITY, t));
            debuffMaterial.SetFloat("_OutlineThickness", Mathf.Lerp(0, MAX_OUTLINE_THICKNESS, t));
            debuffMaterial.SetFloat("_VoronoiPower", Mathf.Lerp(0, MAX_VORONOI_POWER, t));

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure max values are set
        //debuffMaterial.SetFloat("_VignetteIntensity", MAX_VIGNETTE_INTENSITY);
        //debuffMaterial.SetFloat("_VignettePower", MAX_VIGNETTE_POWER);
        //debuffMaterial.SetColor("_VignetteColor", MAX_VIGNETTE_COLOR);
        //debuffMaterial.SetFloat("_VoronoiSpeed", MAX_VORONOI_SPEED);
        //debuffMaterial.SetFloat("_VoronoiIntensity", MAX_VORONOI_INTENSITY);
        //debuffMaterial.SetFloat("_OutlineThickness", MAX_OUTLINE_THICKNESS);
        //debuffMaterial.SetFloat("_VoronoiPower", MAX_VORONOI_POWER);
        //debuffMaterial.SetFloat("_VoronoiDensity", MAX_VORONOI_DENSITY);

        // Wait for debuff duration
        //yield return new WaitForSeconds(duration);

        // Fade Out
        elapsedTime = 0f;
        while (elapsedTime < fadeOutDuration)
        {
            float t = elapsedTime / fadeOutDuration;
            debuffMaterial.SetFloat("_VignetteIntensity", Mathf.Lerp(MAX_VIGNETTE_INTENSITY, 0, t));
            debuffMaterial.SetFloat("_VignettePower", Mathf.Lerp(MAX_VIGNETTE_POWER, 0, t));
            debuffMaterial.SetColor("_VignetteColor", Color.Lerp(MAX_VIGNETTE_COLOR, Color.black, t));
            debuffMaterial.SetFloat("_VoronoiIntensity", Mathf.Lerp(MAX_VORONOI_INTENSITY, 0, t));
            debuffMaterial.SetFloat("_OutlineThickness", Mathf.Lerp(MAX_OUTLINE_THICKNESS, 0, t));
            debuffMaterial.SetFloat("_VoronoiPower", Mathf.Lerp(MAX_VORONOI_POWER, 0, t));

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure all values are reset
        ResetDebuffProperties();
    }
}
