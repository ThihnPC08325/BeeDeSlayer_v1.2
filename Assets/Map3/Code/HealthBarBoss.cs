using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarBoss : MonoBehaviour
{
    [SerializeField] private Slider healthBar;
    [SerializeField] private Slider easeHealthBar;
    private float lerpSpeed = 0.03f;

    [SerializeField] HealthBoss bossHealth;
    void Start()
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
        if (healthBar.value != bossHealth.currentHealth)
        {
            healthBar.value = bossHealth.currentHealth;
        }

        if (healthBar.value != easeHealthBar.value)
        {
            easeHealthBar.value = Mathf.Lerp(easeHealthBar.value, bossHealth.currentHealth, lerpSpeed);
        }
    }
}
