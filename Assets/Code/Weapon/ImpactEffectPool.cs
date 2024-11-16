using System.Collections.Generic;
using UnityEngine;

public class ImpactEffectPool : MonoBehaviour
{
    private static ImpactEffectPool instance;
    public static ImpactEffectPool Instance
    {
        get
        {
            if (instance == null)
            {
                var obj = new GameObject("ImpactEffectPool");
                instance = obj.AddComponent<ImpactEffectPool>();
            }
            return instance;
        }
    }

    public enum EffectType
    {
        SoftBody,
        Explosion,
        Spark,
        Smoke,
        Hole,
        // Thêm các effect khác ở đây nếu cần
    }

    [System.Serializable]
    public class Pool
    {
        [SerializeField] private EffectType _effectType;
        [SerializeField] private GameObject _prefab;
        [SerializeField] private int _size;

        public EffectType EffectType => _effectType;
        public GameObject Prefab => _prefab;
        public int Size => _size;

        // Cache transform để tránh GetComponent liên tục
        private Transform _containerTransform;
        public Transform ContainerTransform
        {
            get => _containerTransform;
            set => _containerTransform = value;
        }
    }

    [SerializeField] private List<Pool> pools = new();
    private readonly Dictionary<EffectType, Queue<GameObject>> poolDictionary = new();
    private readonly Dictionary<EffectType, Pool> poolReference = new();

    [SerializeField] private Transform poolRoot;
    private const string POOL_ROOT_NAME = "---POOL---";

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            InitializePoolSystem();
            DontDestroyOnLoad(poolRoot.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializePoolSystem()
    {
        SetupPoolRoot();
        InitializePools();
    }

    private void SetupPoolRoot()
    {
        poolRoot = poolRoot ? poolRoot : GameObject.Find(POOL_ROOT_NAME)?.transform;
        poolRoot = poolRoot != null ? poolRoot : new GameObject(POOL_ROOT_NAME).transform;
        transform.SetParent(poolRoot);
    }

    private void InitializePools()
    {
        foreach (Pool pool in pools)
        {
            if (pool.Prefab == null)
            {
                Debug.LogError($"Invalid pool configuration detected!");
                continue;
            }

            var container = new GameObject($"Pool_{pool.EffectType}").transform;
            container.SetParent(transform);
            pool.ContainerTransform = container;

            var objectPool = new Queue<GameObject>(pool.Size);
            PrewarmPool(pool, objectPool);

            poolDictionary[pool.EffectType] = objectPool;
            poolReference[pool.EffectType] = pool;
        }
    }

    private void PrewarmPool(Pool pool, Queue<GameObject> objectPool)
    {
        for (int i = 0; i < pool.Size; i++)
        {
            CreateNewInstance(pool, objectPool);
        }
    }

    private void CreateNewInstance(Pool pool, Queue<GameObject> objectPool)
    {
        var obj = Instantiate(pool.Prefab, pool.ContainerTransform);
        obj.SetActive(false);
        objectPool.Enqueue(obj);
    }

    public GameObject SpawnFromPool(EffectType type, Vector3 position, Quaternion rotation)
    {
        if (!ValidatePoolExistence(type)) return null;

        var pool = poolDictionary[type];
        var obj = pool.Count > 0 ? pool.Dequeue() : CreateNewPoolObject(type);

        SetupSpawnedObject(obj, position, rotation);
        return obj;
    }

    private bool ValidatePoolExistence(EffectType type)
    {
        if (poolDictionary.ContainsKey(type)) return true;
        Debug.LogWarning($"Pool with type {type} doesn't exist!");
        return false;
    }

    private GameObject CreateNewPoolObject(EffectType tag)
    {
        var pool = poolReference[tag];
        return Instantiate(pool.Prefab, pool.ContainerTransform);
    }

    private void SetupSpawnedObject(GameObject obj, Vector3 position, Quaternion rotation)
    {
        obj.SetActive(true);
        obj.transform.SetPositionAndRotation(position, rotation);
    }

    public void ReturnToPool(EffectType type, GameObject obj)
    {
        if (!ValidatePoolExistence(type)) return;

        obj.SetActive(false);
        obj.transform.SetParent(poolReference[type].ContainerTransform);
        poolDictionary[type].Enqueue(obj);
    }
}