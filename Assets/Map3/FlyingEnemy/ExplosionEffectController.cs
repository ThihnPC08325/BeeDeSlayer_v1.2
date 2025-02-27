using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionEffectController : MonoBehaviour
{

    [Header("Explosion Settings")]
    [SerializeField] private float destroyDelay = 0.5f;  // Thời gian xóa model sau khi nổ
    [SerializeField] private float explosionRadius = 5f;      // Bán kính tác động của vụ nổ
    [SerializeField] private float explosionDamage = 50f;     // Sát thương gây ra từ vụ nổ

    void Start()
    {
        StartCoroutine(DestroyEffect());
    }

    private IEnumerator DestroyEffect()
    {
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
}
