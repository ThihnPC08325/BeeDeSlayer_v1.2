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
    private void Update()
    {
        UpdateHealth(); // Quản lý mức máu
        UpdateDamageEffect(); // Hiệu ứng fade của damage overlay
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

    private void UpdateHealth()
    {
        // Đảm bảo giá trị health luôn nằm trong phạm vi hợp lệ:
        health = Mathf.Clamp(health, 0, maxHealth);
        UpdateHealthUI(); // Cập nhật giao diện
    }

    private void UpdateDamageEffect()
    {
        // Nếu alpha <= 0 hoặc sức khỏe nhỏ hơn 30 -> không cần xử lý:
        if (damageOverlay.color.a <= 0 || health < 30) return;

        // Tăng thời gian effect timer:
        durationTimer += Time.deltaTime;

        // Nếu quá thời gian hiệu ứng -> giảm dần tempAlpha (hiệu ứng fade):
        if (!(durationTimer > duration)) return;
        float tempAlpha = damageOverlay.color.a;
        tempAlpha -= Time.deltaTime * fadeSpeed;
        tempAlpha = Mathf.Clamp(tempAlpha, 0, 1);

        damageOverlay.color = new Color(damageOverlay.color.r, damageOverlay.color.g, damageOverlay.color.b,
            tempAlpha);
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
    public void TakeMeteorDamage(float explosionDamage, float dotDamage, int dotTicks, float dotInterval)
    {
        Debug.Log($"🔥 Player trúng thiên thạch! Sát thương nổ: {explosionDamage}, DOT: {dotDamage} x {dotTicks} lần");

        TakeDamage(explosionDamage, 0);  // Gây sát thương ngay lập tức
        ApplyDOT(dotDamage, dotTicks, dotInterval);  // Gây sát thương theo thời gian
    }

    public void RestoreHealth(float heal)
    {
        float healthBeforePickup = health;
        health = Mathf.Min(health + heal, maxHealth);
        float healthActuallyHeal = health - healthBeforePickup;
        lerpTimer = 0f;
    }
}