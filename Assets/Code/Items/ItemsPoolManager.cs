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
            DontDestroyOnLoad(gameObject);
            SetupPoolStructure();
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
    }

    private void InitializePools()
    {
        poolDictionary = new Dictionary<PoolType, Queue<GameObject>>();
        containerDictionary = new Dictionary<PoolType, Transform>();

        foreach (Pool pool in pools)
        {
            CreatePool(pool);
        }
    }

    private void CreatePool(Pool pool)
    {
        Transform container = new GameObject($"Pool_{pool.Type}").transform;
        container.parent = poolRoot;

        Queue<GameObject> objectPool = new Queue<GameObject>();
        for (int i = 0; i < pool.Size; i++)
        {
            objectPool.Enqueue(CreateNewPoolObject(pool.Prefab, container));
        }

        containerDictionary[pool.Type] = container;
        poolDictionary[pool.Type] = objectPool;
    }

    private GameObject CreateNewPoolObject(GameObject prefab, Transform parent)
    {
        GameObject obj = Instantiate(prefab, parent);
        obj.SetActive(false);
        return obj;
    }

    public GameObject SpawnFromPool(PoolType type, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.TryGetValue(type, out var pool))
        {
            Debug.LogWarning($"Pool with type {type} does not exist! 🚫");
            return null;
        }

        GameObject objectToSpawn = pool.Count > 0
            ? pool.Dequeue()
            : CreateNewPoolObject(pools.Find(p => p.Type == type).Prefab, containerDictionary[type]);

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.SetPositionAndRotation(position, rotation);
        return objectToSpawn;
    }

    public void ReturnToPool(PoolType type, GameObject obj)
    {
        if (!poolDictionary.TryGetValue(type, out var pool))
        {
            Debug.LogWarning($"Pool with type {type} does not exist! 🚫");
            return;
        }

        obj.SetActive(false);
        obj.transform.parent = containerDictionary[type];
        pool.Enqueue(obj);
    }
}