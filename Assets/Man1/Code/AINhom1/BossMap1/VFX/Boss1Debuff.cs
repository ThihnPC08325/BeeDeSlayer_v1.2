using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerDebuffEffect : MonoBehaviour
{
    [SerializeField] private Material debuffMaterial;
    [SerializeField] private float fadeInDuration = 0.5f;  // Time to reach max intensity
    [SerializeField] private float fadeOutDuration = 2f;   // Time to fade out after duration
    private RawImage debuffOverlay;
    private float maxIntensity = 2.0f;

    void Awake()
    {
        debuffOverlay = GetComponent<RawImage>();
        debuffMaterial.SetFloat("_VoronoiIntensity", 0f);
        debuffMaterial.SetFloat("_VignetteIntensity", 0f);
        debuffMaterial.SetFloat("_VignettePower", 0f);
    }
    void Start()
    {
        debuffOverlay = GetComponent<RawImage>();

        if (debuffOverlay == null)
        {
            return;
        }
        debuffMaterial.SetFloat("_VoronoiIntensity", 0f);
        debuffMaterial.SetFloat("_VignetteIntensity", 0f);
        debuffMaterial.SetFloat("_VignettePower", 0f);
        debuffOverlay.material = debuffMaterial;
         // Ensure starting intensity is 0
    }

    public void ApplyDebuff(float duration)
    {
        StartCoroutine(DebuffEffect(duration));
    }

    private IEnumerator DebuffEffect(float duration)
    {
        // Fade In
        float elapsedTime = 0f;
        while (elapsedTime < fadeInDuration)
        {
            float intensity = Mathf.Lerp(0, maxIntensity, elapsedTime / fadeInDuration);
            debuffMaterial.SetFloat("_VoronoiIntensity", intensity);
            debuffMaterial.SetFloat("_VignetteIntensity", 1.6f);
            debuffMaterial.SetFloat("_VignettePower", 3.2f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        debuffMaterial.SetFloat("_VoronoiIntensity", maxIntensity); // Ensure max value is set

        // Wait for debuff duration
        yield return new WaitForSeconds(duration);

        // Fade Out
        elapsedTime = 0f;
        while (elapsedTime < fadeOutDuration)
        {
            float intensity = Mathf.Lerp(maxIntensity, 0, elapsedTime / fadeOutDuration);
            debuffMaterial.SetFloat("_VoronoiIntensity", intensity);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        debuffMaterial.SetFloat("_VoronoiIntensity", 0f); // Ensure it's fully off
        debuffMaterial.SetFloat("_VignetteIntensity", 0f);
        debuffMaterial.SetFloat("_VignettePower", 0f);
    }
}
