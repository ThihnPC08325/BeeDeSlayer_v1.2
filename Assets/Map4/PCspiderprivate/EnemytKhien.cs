using System.Collections;
using UnityEngine;

public class EnemytKhien : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab; 
    [SerializeField] private float spawnInterval = 10f; 
    [SerializeField] private Transform spawnPoint;

    private void Start()
    {
       
        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        while (true) 
        {
            
            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

            
            StartCoroutine(DestroyAfterDelay(enemy, 2f));

            
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private IEnumerator DestroyAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(obj); 
    }
}