using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public interface IPooledObject
    {
        void OnObjectSpawn();
    }

    public enum EnemyType
    {
        None = 0,
        Shooting,
        Melee,
        Skeleton,
<<<<<<< Updated upstream
        Spider,
        Bat,
        Tanker,

        // Thêm các loại enemy khác
=======
        Spider
>>>>>>> Stashed changes
    }

    [System.Serializable]
    public class Pool
    {
        public EnemyType enemyType;
        public GameObject prefab;
        public int initialSize;
        public int maxSize = 100;
    }

    [SerializeField] private List<Pool> pools;
    [SerializeField] private Transform poolRoot;
    private List<Queue<GameObject>> objectPools;

    private void Awake()
    {
        objectPools = new List<Queue<GameObject>>();
        InitializePools();
    }

    private void InitializePools()
    {
        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();
            for (int i = 0; i < pool.initialSize; i++)
            {
                GameObject obj = CreateNewPoolObject(pool.prefab, poolRoot, i);
                objectPool.Enqueue(obj);
            }
            objectPools.Add(objectPool);
        }
    }

    private GameObject CreateNewPoolObject(GameObject prefab, Transform parent, int index)
    {
        GameObject obj = Instantiate(prefab, parent);
        obj.name = $"{prefab.name}_Pool_{index}";
        obj.SetActive(false);
        return obj;
    }

    public GameObject SpawnFromPool(EnemyType enemyType, Vector3 position, Quaternion rotation)
    {
        int typeIndex = (int)enemyType;
        if (typeIndex < 0 || typeIndex >= objectPools.Count)
        {
            Debug.LogWarning($"Pool for enemy type {enemyType} doesn't exist! 🚫");
            return null;
        }

        Queue<GameObject> pool = objectPools[typeIndex];
        GameObject obj = GetInactiveObject(pool);
        if (obj != null)
        {
            SetupSpawnedObject(obj, position, rotation);
        }

        return obj;
    }

    private GameObject GetInactiveObject(Queue<GameObject> pool)
    {
        GameObject obj = null;
        if (pool.Count > 0)
        {
            obj = pool.Dequeue();
        }
        return obj;
    }

    private void SetupSpawnedObject(GameObject obj, Vector3 position, Quaternion rotation)
    {
        obj.SetActive(true);
        obj.transform.SetPositionAndRotation(position, rotation);
    }

    public void ReturnToPool(GameObject obj, EnemyType enemyType)
    {
        int typeIndex = (int)enemyType;
        if (typeIndex >= 0 && typeIndex < objectPools.Count)
        {
            obj.SetActive(false);
            objectPools[typeIndex].Enqueue(obj);
        }
    }
}