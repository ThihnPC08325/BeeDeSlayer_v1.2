using System.Collections.Generic;
using UnityEngine;

public class ItemsPoolManager : MonoBehaviour
{
    public static ItemsPoolManager Instance { get; private set; }

    public enum PoolType
    {
        None,
        Health,
        Ammo,
        // Thêm các loại khác tùy nhu cầu
    }

    [System.Serializable]
    public class Pool
    {
        [SerializeField] private PoolType _type;
        [SerializeField] private GameObject _prefab;
        [SerializeField] private int _size;

        public PoolType Type => _type;
        public GameObject Prefab => _prefab;
        public int Size => _size;
    }

    [SerializeField] private List<Pool> pools;
    [SerializeField] private Transform poolRoot;

    private Dictionary<PoolType, Queue<GameObject>> poolDictionary;
    private Dictionary<PoolType, Transform> containerDictionary;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            SetupPoolStructure();
            DontDestroyOnLoad(poolRoot.gameObject);
            InitializePools();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void SetupPoolStructure()
    {
        poolRoot ??= new GameObject("---POOL---").transform;
        transform.parent = poolRoot;
    }

    private void InitializePools()
    {
        poolDictionary = new Dictionary<PoolType, Queue<GameObject>>();
        containerDictionary = new Dictionary<PoolType, Transform>();

        foreach (Pool pool in pools)
        {
            GameObject poolContainer = new GameObject($"Pool_{pool.Type}");
            poolContainer.transform.parent = transform;

            Queue<GameObject> objectPool = new Queue<GameObject>();
            containerDictionary[pool.Type] = poolContainer.transform;

            for (int i = 0; i < pool.Size; i++)
            {
                CreateNewPoolObject(pool, objectPool, poolContainer.transform);
            }

            poolDictionary[pool.Type] = objectPool;
        }
    }

    private void CreateNewPoolObject(Pool pool, Queue<GameObject> objectPool, Transform parent)
    {
        GameObject obj = Instantiate(pool.Prefab, parent);
        obj.SetActive(false);
        objectPool.Enqueue(obj);
    }

    public GameObject SpawnFromPool(PoolType type, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(type))
        {
            Debug.LogWarning($"Pool with type {type} doesn't exist! 🚫");
            return null;
        }

        Queue<GameObject> pool = poolDictionary[type];
        GameObject objectToSpawn;

        if (pool.Count == 0)
        {
            Pool originalPool = pools.Find(p => p.Type == type);
            objectToSpawn = Instantiate(originalPool.Prefab, containerDictionary[type]);
        }
        else
        {
            objectToSpawn = pool.Dequeue();
        }

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.SetPositionAndRotation(position, rotation);
        return objectToSpawn;
    }

    public void ReturnToPool(PoolType type, GameObject obj)
    {
        if (!poolDictionary.ContainsKey(type))
        {
            Debug.LogWarning($"Pool with type {type} doesn't exist! 🚫");
            return;
        }

        obj.transform.parent = containerDictionary[type];
        obj.SetActive(false);
        poolDictionary[type].Enqueue(obj);
    }
}