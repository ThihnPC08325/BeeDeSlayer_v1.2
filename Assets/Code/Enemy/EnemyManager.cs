using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public interface IPooledObject
    {
        void OnObjectSpawn();
    }

    private static EnemyManager instance;
    public static EnemyManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<EnemyManager>();
                if (instance == null)
                {
                    GameObject go = new("EnemyPool");
                    instance = go.AddComponent<EnemyManager>();
                }
            }
            return instance;
        }
    }

    public enum EnemyType
    {
        None = 0,
        Shooting,
        Melee,
        Skeleton,
        Enemy,
        // Thêm các loại enemy khác
    }

    [System.Serializable]
    public class Pool
    {
        [SerializeField] private EnemyType _enemyType;
        [SerializeField] private GameObject _prefab;
        [SerializeField] private int _initialSize;
        [SerializeField] private int _maxSize = 100;

        public EnemyType EnemyType => _enemyType;
        public GameObject Prefab => _prefab;
        public int InitialSize => _initialSize;
        public int MaxSize => _maxSize;
    }

    [SerializeField] private List<Pool> pools;
    [SerializeField] private Transform poolRoot;

    private Dictionary<EnemyType, Queue<GameObject>> poolDictionary;
    private Dictionary<EnemyType, Transform> containerDictionary; // Cache container transforms

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            Initialize();
            DontDestroyOnLoad(poolRoot.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Initialize()
    {
        poolDictionary = new();
        containerDictionary = new();
        SetupPoolStructure();
        InitializePools();
    }

    private void SetupPoolStructure()
    {
        poolRoot = poolRoot ?? CreateOrFindPoolRoot();
        Transform enemyPoolContainer = CreateOrFindContainer("EnemyPool", poolRoot);
        transform.SetParent(enemyPoolContainer);
    }

    private Transform CreateOrFindPoolRoot()
    {
        GameObject root = GameObject.Find("---POOL---") ?? new GameObject("---POOL---");
        return root.transform;
    }

    private Transform CreateOrFindContainer(string name, Transform parent)
    {
        Transform container = parent.Find(name);
        if (container == null)
        {
            container = new GameObject(name).transform;
            container.SetParent(parent);
        }
        return container;
    }

    private void InitializePools()
    {
        foreach (Pool pool in pools)
        {
            if (pool.Prefab == null)
            {
                Debug.LogError($"Invalid pool configuration for {pool.EnemyType}! 🔴");
                continue;
            }

            Transform container = CreateOrFindContainer(pool.EnemyType.ToString(), transform);
            containerDictionary[pool.EnemyType] = container;

            Queue<GameObject> objectPool = new Queue<GameObject>();
            CreateInitialObjects(pool, objectPool, container);
            poolDictionary[pool.EnemyType] = objectPool;
        }
    }

    private void CreateInitialObjects(Pool pool, Queue<GameObject> objectPool, Transform container)
    {
        for (int i = 0; i < pool.InitialSize; i++)
        {
            GameObject obj = CreateNewPoolObject(pool.Prefab, container, i);
            objectPool.Enqueue(obj);
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
        if (!poolDictionary.TryGetValue(enemyType, out Queue<GameObject> pool))
        {
            Debug.LogWarning($"Pool for enemy type {enemyType} doesn't exist! 🚫");
            return null;
        }

        GameObject obj = GetInactiveObject(pool, enemyType);
        if (obj != null)
        {
            SetupSpawnedObject(obj, position, rotation);
        }

        return obj;
    }

    private GameObject GetInactiveObject(Queue<GameObject> pool, EnemyType enemyType)
    {
        Pool originalPool = pools.Find(p => p.EnemyType == enemyType);
        GameObject obj;

        if (pool.Count == 0 && originalPool != null)
        {
            if (pool.Count < originalPool.MaxSize)
            {
                obj = CreateNewPoolObject(originalPool.Prefab, containerDictionary[enemyType], pool.Count);
            }
            else
            {
                obj = pool.Dequeue();
            }
        }
        else
        {
            obj = pool.Dequeue();
        }

        return obj;
    }

    private void SetupSpawnedObject(GameObject obj, Vector3 position, Quaternion rotation)
    {
        obj.SetActive(true);
        obj.transform.SetPositionAndRotation(position, rotation);

        if (obj.TryGetComponent(out IPooledObject pooledObj))
        {
            pooledObj.OnObjectSpawn();
        }
    }

    public void ReturnToPool(GameObject obj, EnemyType enemyType)
    {
        if (poolDictionary.TryGetValue(enemyType, out Queue<GameObject> pool))
        {
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }
}