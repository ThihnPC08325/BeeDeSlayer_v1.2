using UnityEngine;
using System.Collections;

public class BrokenFlyDebuff : MonoBehaviour
{
    [SerializeField] private Material debuffMaterial;
    [SerializeField] private float fadeOutDuration = 2f; // How long it takes to fade out
    [SerializeField] private float dotDuration = 3f; // Total DOT time

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerDebuffEffect player = other.GetComponent<PlayerDebuffEffect>();
            if (player != null)
            {
                Debug.Log("Debuff Hit");
                player.ApplyDebuff(dotDuration);
            }
        }
    }
    private float maxIntensity = 2.0f;
    private int dotHits = 3;
    private float interval;

    public void ApplyDebuff(float dotDamage, float duration)
    {
        interval = duration / dotHits;
        StartCoroutine(DebuffEffect(dotDamage));
    }

    private IEnumerator DebuffEffect(float dotDamage)
    {
        float time = 0f;
        float intensity = 0f;

        // Debuff starts
        while (time < dotHits * interval)
        {
            int hitNumber = Mathf.FloorToInt(time / interval);
            intensity = Mathf.Lerp(intensity, maxIntensity * (hitNumber + 1) / dotHits, 0.5f);
            debuffMaterial.SetFloat("VoronoiIntensity", intensity);

            yield return new WaitForSeconds(interval); // Wait for next DOT hit
            time += interval;
        }

        // Fade out effect
        StartCoroutine(FadeOutEffect());
    }

    private IEnumerator FadeOutEffect()
    {
        float time = 0f;
        float startIntensity = debuffMaterial.GetFloat("VoronoiIntensity");

        while (time < fadeOutDuration)
        {
            float newIntensity = Mathf.Lerp(startIntensity, 0, time / fadeOutDuration);
            debuffMaterial.SetFloat("VoronoiIntensity", newIntensity);
            time += Time.deltaTime;
            yield return null;
        }

        debuffMaterial.SetFloat("VoronoiIntensity", 0);
    }
}
