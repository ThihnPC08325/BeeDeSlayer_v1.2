using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float health;
    [SerializeField] private float armor;
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float maxArmor = 50f; // Giáp tối đa
    [SerializeField] private float chipSpeed = 2f;
    [SerializeField] private Image frontHealthBar;
    [SerializeField] private Image backHealthBar;
    [SerializeField] private Image armorBar; // Thanh giáp mới
    [SerializeField] private Image DamagaOverlay;
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
        armor = maxArmor; // Bắt đầu với giáp đầy
    }

    // Start is called before the first frame update
    void Start()
    {
        DamagaOverlay.color = new Color(DamagaOverlay.color.r, DamagaOverlay.color.g, DamagaOverlay.color.b, 0);
    }

    // Update is called once per frame
    void Update()
    {
        health = Mathf.Clamp(health, 0, maxHealth);
        armor = Mathf.Clamp(armor, 0, maxArmor); // Đảm bảo giáp không vượt quá giới hạn
        UpdateHealthUI();
        if (DamagaOverlay.color.a > 0)
        {
            if (health < 30)
            {
                return;
            }
            durationTimer += Time.deltaTime;
            if (durationTimer > duration)
            {
                float tempAlpha = DamagaOverlay.color.a;
                tempAlpha = Time.deltaTime * fadeSpeed;
                DamagaOverlay.color = new Color(DamagaOverlay.color.r, DamagaOverlay.color.g, DamagaOverlay.color.b, tempAlpha);
            }
        }

    }

    public void UpdateHealthUI()
    {
        float fillFrontHealth = frontHealthBar.fillAmount;
        float fillBackHealth = backHealthBar.fillAmount;
        float fillArmor = armorBar.fillAmount; // Cập nhật thanh giáp
        float hFraction = health / maxHealth;
        float aFraction = armor / maxArmor;

        armorBar.fillAmount = aFraction;


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

    public void TakeDamage(float damage, float penetration)
    {

        if (armor > 0)
        {
            // Nếu giáp còn, trừ giáp trước
            float damageToArmor = Mathf.Min(damage, armor);
            armor -= damageToArmor;
            damage -= damageToArmor;
        }

        // Nếu giáp đã hết, trừ vào máu
        if (damage > 0)
        {
            health = Mathf.Max(health - damage, 0);
        }

        float HealthBeforeDamage = health;
        float finalDamage = defenseSystem.CalculateDamage(damage, armor, penetration);
        health = Mathf.Max(health - finalDamage, 0);
        float healthActuallyDamage = health - HealthBeforeDamage;
        lerpTimer = 0f;
        durationTimer = 0f;
        DamagaOverlay.color = new Color(DamagaOverlay.color.r, DamagaOverlay.color.g, DamagaOverlay.color.b, 1);
    }

    public void RestoreHealth(float heal)
    {
        float HealthBeforePickup = health;
        health = Mathf.Min(health + heal, maxHealth);
        float healthActuallyHeal = health - HealthBeforePickup;
        lerpTimer = 0f;
    }

    public void RestoreArmor(float armorPoints)
    {
        armor = Mathf.Min(armor + armorPoints, maxArmor);
        lerpTimer = 0f;
    }
}