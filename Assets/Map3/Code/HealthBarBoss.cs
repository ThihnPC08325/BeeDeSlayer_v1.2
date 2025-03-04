using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarBoss : MonoBehaviour
{
    [SerializeField] private Slider healthBar;
    [SerializeField] private Slider easeHealthBar;
    private const float LerpSpeed = 0.3f;

    [SerializeField] private HealthBoss bossHealth;

    private void Start()
    {
        healthBar.maxValue = bossHealth.currentHealth;
        easeHealthBar.maxValue = bossHealth.currentHealth;
    }

    public void HealthEnemy(float damage)
    {
        Debug.Log("Tru mau");
        bossHealth.currentHealth -= damage;
    }
    private void Update()
    {
        UpdateSlider();
    }
    private void UpdateSlider()
    {
        if (!Mathf.Approximately(healthBar.value, bossHealth.currentHealth))
        {
            healthBar.value = bossHealth.currentHealth;
        }

        if (!Mathf.Approximately(healthBar.value, easeHealthBar.value))
        {
            easeHealthBar.value = Mathf.Lerp(easeHealthBar.value, bossHealth.currentHealth, LerpSpeed);
        }
    }
}
