using System;
using System.Collections;
using UnityEngine;

public class GuardianHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float health = 100f;
    public float maxHealth = 100f;

    [SerializeField] private ItemDropManager itemDropManager;
    [SerializeField] private GameObject beeWorkerPrefab;
    [SerializeField] private Transform summonPoint;
    [SerializeField] private int maxBeeWorkers = 3;

    private int currentBeeWorkers = 0;
    private BoxCollider boxCollider;

    // Sự kiện thay đổi sức khỏe
    public event Action<float, float> OnHealthChanged;

    // Sự kiện khi Guardian chết
    public event Action OnGuardianDeath;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        itemDropManager = GetComponent<ItemDropManager>();
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        // Gọi sự kiện khi sức khỏe thay đổi
        OnHealthChanged?.Invoke(health, maxHealth);

        // Kiểm tra nếu sức khỏe dưới 50% và chưa đủ số BeeWorkers
        if (health <= 50f && currentBeeWorkers < maxBeeWorkers)
        {
            SummonBeeWorker();
        }

        // Nếu sức khỏe = 0, gọi phương thức Die() ngay lập tức
        if (health <= 0f)
        {
            health = 0f;
            Die();  // Gọi ngay Die() khi máu = 0
        }
    }

    private void SummonBeeWorker()
    {
        Instantiate(beeWorkerPrefab, summonPoint.position, Quaternion.identity);
        currentBeeWorkers++;
    }

    private void Die()
    {
        // Gọi sự kiện khi Guardian chết
        OnGuardianDeath?.Invoke();

        Debug.Log("Die() method called! Hiding the Guardian.");

        // Tắt collider để ngừng va chạm
        boxCollider.enabled = false;

        // Gọi drop vật phẩm nếu có
        itemDropManager?.TryDropLoot(transform.position);

        // Ẩn đối tượng thay vì xóa
        gameObject.SetActive(false);  // Ẩn đối tượng khi chết
    }

    // Thêm phương thức StartHealing để hồi phục sức khỏe
    public void StartHealing(float healAmountPerSecond)
    {
        StartCoroutine(HealingCoroutine(healAmountPerSecond));
    }

    // Coroutine hồi phục sức khỏe
    private IEnumerator HealingCoroutine(float healAmountPerSecond)
    {
        while (health < maxHealth)
        {
            health += healAmountPerSecond;
            if (health > maxHealth)
            {
                health = maxHealth;  // Đảm bảo không vượt quá maxHealth
            }

            // Gọi sự kiện khi sức khỏe thay đổi trong khi hồi phục
            OnHealthChanged?.Invoke(health, maxHealth);

            yield return new WaitForSeconds(1f);  // Hồi phục mỗi giây
        }
    }
}
