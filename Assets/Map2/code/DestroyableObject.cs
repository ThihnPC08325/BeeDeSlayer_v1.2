using UnityEngine;
using UnityEngine.Serialization;

public class BreakableCrate : MonoBehaviour
{
    [SerializeField] private GameObject[] lootItems; // Danh sách vật phẩm có thể rơi ra
    //[SerializeField] private GameObject brokenCratePrefab; // Prefab thùng vỡ
    [SerializeField] private float maxHealth = 50; // Máu của thùng
    [SerializeField] private Transform spawnpoint;
    private float _currentHealth;

    private void Start()
    {
        _currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        _currentHealth -= damage;
        if (_currentHealth <= 0)
        {
            BreakCrate();
        }
    }

    private void BreakCrate()
    {
        //if (brokenCratePrefab != null)
        //{
        //    Instantiate(brokenCratePrefab, transform.position, transform.rotation);
        //}

        DropLoot();
        Destroy(gameObject);
    }

    private void DropLoot()
    {
        if (lootItems.Length <= 0) return;
        int randomIndex = Random.Range(0, lootItems.Length);
        Instantiate(lootItems[randomIndex], spawnpoint.position, Quaternion.identity);
    }
}
