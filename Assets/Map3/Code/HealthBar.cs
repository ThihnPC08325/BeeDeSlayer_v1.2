using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider healthBar;
    [SerializeField] private Slider easeHealthBar;
    private float lerpSpeed = 0.03f;

    [SerializeField] EnemyHealth enemyHealth;
    void Start()
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
        if (healthBar.value != enemyHealth.currentHealth)
        {
            healthBar.value = enemyHealth.currentHealth;
        }

        if (healthBar.value != easeHealthBar.value)
        {
            easeHealthBar.value = Mathf.Lerp(easeHealthBar.value, enemyHealth.currentHealth, lerpSpeed);
        }
    }
}

