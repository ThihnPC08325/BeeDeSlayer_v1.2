using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class FullscreenPassController : MonoBehaviour
{
    [SerializeField] private ScriptableRendererFeature map2FullscreenEffect;

    private void Start()    
    {
        map2FullscreenEffect.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            map2FullscreenEffect.SetActive(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            map2FullscreenEffect.SetActive(false);
        }
    }
}
