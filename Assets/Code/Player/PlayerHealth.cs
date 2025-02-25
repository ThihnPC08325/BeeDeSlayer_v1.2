using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Serialization;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float health;
    [SerializeField] private float defense;
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float maxDefense = 30f;
    [SerializeField] private float chipSpeed = 2f;
    [SerializeField] private Image frontHealthBar;
    [SerializeField] private Image backHealthBar;
    [SerializeField] private Image damageOverlay;
    [SerializeField] private float duration;
    [SerializeField] private float fadeSpeed;

    private float durationTimer;
    private float lerpTimer;
    private DefenseSystem defenseSystem;

    private void OnEnable()
    {
        GameEvents.OnHealthPickup += RestoreHealth;
        GameEvents.OnPlayerHit += TakeDamage;
    }

    private void OnDisable()
    {
        GameEvents.OnHealthPickup -= RestoreHealth;
        GameEvents.OnPlayerHit -= TakeDamage;
    }

    private void Awake()
    {
        defenseSystem = GetComponent<DefenseSystem>();
        health = maxHealth;
        defense = maxDefense;
    }

    // Start is called before the first frame update
    private void Start()
    {
        damageOverlay.color = new Color(damageOverlay.color.r, damageOverlay.color.g, damageOverlay.color.b, 0);
    }

    // Update is called once per frame
    void Update()
    {
        health = Mathf.Clamp(health, 0, maxHealth);
        UpdateHealthUI();
        if (!(damageOverlay.color.a > 0)) return;
        if (health < 30)
        {
            return;
        }
        durationTimer += Time.deltaTime;
        if (!(durationTimer > duration)) return;
        float tempAlpha = damageOverlay.color.a;
        tempAlpha = Time.deltaTime * fadeSpeed;
        damageOverlay.color = new Color(damageOverlay.color.r, damageOverlay.color.g, damageOverlay.color.b, tempAlpha);

    }

    public void UpdateHealthUI()
    {
        float fillFrontHealth = frontHealthBar.fillAmount;
        float fillBackHealth = backHealthBar.fillAmount;
        float hFraction = health / maxHealth;
        if (fillBackHealth > hFraction)
        {
            frontHealthBar.fillAmount = hFraction;
            backHealthBar.color = Color.red;
            lerpTimer += Time.deltaTime;
            float percent = lerpTimer / chipSpeed;
            percent = percent * percent; //Cho hiệu ứng đẹp hơn
            backHealthBar.fillAmount = Mathf.Lerp(fillBackHealth, hFraction, percent);
        }

        if (fillFrontHealth < hFraction)
        {
            backHealthBar.color = Color.green;
            backHealthBar.fillAmount = hFraction;
            lerpTimer += Time.deltaTime;
            float percent = lerpTimer / chipSpeed;
            percent = percent * percent; //Cho hiệu ứng đẹp hơn
            frontHealthBar.fillAmount = Mathf.Lerp(fillFrontHealth, backHealthBar.fillAmount, percent);
        }
    }
    public void ApplyDOT(float dotDamage, int dotTicks, float dotInterval)
    {
        StartCoroutine(DOTCoroutine(dotDamage, dotTicks, dotInterval));
    }

    private IEnumerator DOTCoroutine(float dotDamage, int dotTicks, float dotInterval)
    {

        for (int i = 0; i < dotTicks; i++)
        {
            yield return new WaitForSeconds(dotInterval);
            TakeDamage(dotDamage, 0);
        }
    }
    public void TakeDamage(float damage, float penetration)
    {
        float healthBeforeDamage = health;
        float finalDamage = defenseSystem.CalculateDamage(damage, defense, penetration);
        health = Mathf.Max(health - finalDamage, 0);
        float healthActuallyDamage = health - healthBeforeDamage;
        lerpTimer = 0f;
        durationTimer = 0f;
        damageOverlay.color = new Color(damageOverlay.color.r, damageOverlay.color.g, damageOverlay.color.b, 1);
    }

    public void RestoreHealth(float heal)
    {
        float healthBeforePickup = health;
        health = Mathf.Min(health + heal, maxHealth);
        float healthActuallyHeal = health - healthBeforePickup;
        lerpTimer = 0f;
    }
}