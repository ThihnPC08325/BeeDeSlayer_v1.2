using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarLazer : MonoBehaviour
{
    [SerializeField] private Slider healthBar;
    [SerializeField] private Slider easeHealthBar;
    private float lerpSpeed = 0.03f;

    [SerializeField] LazerEnemyHealth enemyHealth;

    private void Start()
    {
        healthBar.maxValue = enemyHealth.currentHealth;
        easeHealthBar.maxValue = enemyHealth.currentHealth;
    }

    public void HealthEnemy(float damage)
    {
        Debug.Log("Tru mau");
        enemyHealth.currentHealth -= damage;
    }
    private void Update()
    {
        UpdateSlider();
    }
    private void UpdateSlider()
    {
        if (!Mathf.Approximately(healthBar.value, enemyHealth.currentHealth))
        {
            healthBar.value = enemyHealth.currentHealth;
        }

        if (!Mathf.Approximately(healthBar.value, easeHealthBar.value))
        {
            easeHealthBar.value = Mathf.Lerp(easeHealthBar.value, enemyHealth.currentHealth, lerpSpeed);
        }
    }
}
