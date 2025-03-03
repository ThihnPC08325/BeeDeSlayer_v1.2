using UnityEngine;

public class BreakableCrate : MonoBehaviour
{
    [SerializeField] private GameObject[] lootItems; // Danh sách vật phẩm có thể rơi ra
    //[SerializeField] private GameObject brokenCratePrefab; // Prefab thùng vỡ
    [SerializeField] private float maxHealth = 50; // Máu của thùng
    [SerializeField] private Transform Spawnpoint;
    private float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            BreakCrate();
        }
    }

    void BreakCrate()
    {
        //if (brokenCratePrefab != null)
        //{
        //    Instantiate(brokenCratePrefab, transform.position, transform.rotation);
        //}

        DropLoot();
        Destroy(gameObject);
    }
    void DropLoot()
    {
        if (lootItems.Length > 0)
        {
            int randomIndex = Random.Range(0, lootItems.Length);
            Instantiate(lootItems[randomIndex], Spawnpoint.position, Quaternion.identity);
        }
    }
}
