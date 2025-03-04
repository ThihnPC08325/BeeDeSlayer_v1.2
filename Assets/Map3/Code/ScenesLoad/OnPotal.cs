using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnPotal : MonoBehaviour
{
    [SerializeField] private HealthBoss healthBoss;
    [SerializeField] private GameObject potal;
    private void Start()
    {
        potal.SetActive(false);
        StartCoroutine(OnPortal());
    }

    private IEnumerator OnPortal()
    {
        while (true)
        {
            if (healthBoss.currentHealth <= 0)
            {
                potal.SetActive(true);
            }
            yield return new WaitForSeconds(1f);
        }
    }
}