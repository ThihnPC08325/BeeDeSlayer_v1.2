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
        Spider,
        BrokenFly,
        Worm,
        BlueDragon,
        Redd,
        Purple,
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
    private Dictionary<EnemyType, Queue<GameObject>> poolDictionary;
    private Dictionary<EnemyType, Pool> poolConfigs;

    private void Awake()
    {
        poolDictionary = new Dictionary<EnemyType, Queue<GameObject>>();
        poolConfigs = new Dictionary<EnemyType, Pool>();
        InitializePoolConfigs();
    }

    private void InitializePoolConfigs()
    {
        foreach (Pool pool in pools)
        {
            poolConfigs[pool.enemyType] = pool;
            poolDictionary[pool.enemyType] = new Queue<GameObject>();
            // Chỉ khởi tạo một số lượng nhỏ ban đầu
            PrewarmPool(pool.enemyType, Mathf.Min(5, pool.initialSize));
        }
    }

    private void PrewarmPool(EnemyType type, int count)
    {
        for (int i = 0; i < count; i++)
        {
            CreateNewObjectInPool(type);
        }
    }

    private GameObject CreateNewObjectInPool(EnemyType type)
    {
        Pool config = poolConfigs[type];
        GameObject obj = CreateNewPoolObject(config.prefab, poolRoot, poolDictionary[type].Count);
        poolDictionary[type].Enqueue(obj);
        return obj;
    }

    private GameObject CreateNewPoolObject(GameObject prefab, Transform parent, int index)
    {
        if (prefab == null)
        {
            Debug.LogError("Prefab is null! Cannot instantiate object.");
            return null;
        }

        GameObject obj = Instantiate(prefab, parent);
        obj.name = $"{prefab.name}_Pool_{index}";
        obj.SetActive(false);
        return obj;
    }

    public GameObject SpawnFromPool(EnemyType enemyType, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(enemyType) || poolDictionary[enemyType] == null)
        {
            Debug.LogWarning($"Pool for enemy type {enemyType} doesn't exist or is null! 🚫");
            return null;
        }

        Queue<GameObject> pool = poolDictionary[enemyType];
        Pool config = poolConfigs[enemyType];

        GameObject obj = GetOrCreatePooledObject(pool, enemyType, config);
        if (obj != null)
        {
            SetupSpawnedObject(obj, position, rotation);
        }

        return obj;
    }

    private GameObject GetOrCreatePooledObject(Queue<GameObject> pool, EnemyType type, Pool config)
    {
        if (pool.Count > 0)
        {
            return pool.Dequeue();
        }

        if (pool.Count < config.maxSize)
        {
            return CreateNewObjectInPool(type);
        }

        Debug.LogWarning($"Pool for {type} has reached its maximum size of {config.maxSize}! 🚫");
        return null;
    }

    private void SetupSpawnedObject(GameObject obj, Vector3 position, Quaternion rotation)
    {
        obj.SetActive(true);
        obj.transform.SetPositionAndRotation(position, rotation);
    }

    public void ReturnToPool(GameObject obj, EnemyType enemyType)
    {
        if (!poolDictionary.ContainsKey(enemyType) || obj == null)
            return;

        obj.SetActive(false);
        poolDictionary[enemyType].Enqueue(obj);
    }
}