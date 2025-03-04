using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ImpactEffectPool : MonoBehaviour
{
    private static ImpactEffectPool _instance;
    public static ImpactEffectPool Instance
    {
        get
        {
            if (_instance == null)
            {
                var obj = new GameObject("ImpactEffectPool");
                _instance = obj.AddComponent<ImpactEffectPool>();
            }
            return _instance;
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
        [SerializeField] private EffectType effectType;
        [SerializeField] private GameObject prefab;
        [SerializeField] private int size;

        public EffectType EffectType => effectType;
        public GameObject Prefab => prefab;
        public int Size => size;

        // Cache transform để tránh GetComponent liên tục
        private Transform _containerTransform;
        public Transform ContainerTransform
        {
            get => _containerTransform;
            set => _containerTransform = value;
        }
    }

    [SerializeField] private List<Pool> pools = new();
    private readonly Dictionary<EffectType, Queue<GameObject>> _poolDictionary = new();
    private readonly Dictionary<EffectType, Pool> _poolReference = new();

    [SerializeField] private Transform poolRoot;
    private const string PoolRootName = "---POOL---";

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            InitializePoolSystem();
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
        poolRoot = poolRoot ? poolRoot : GameObject.Find(PoolRootName)?.transform;
        poolRoot = poolRoot != null ? poolRoot : new GameObject(PoolRootName).transform;
        transform.SetParent(poolRoot);
    }

    private void InitializePools()
    {
        StartCoroutine(InitializePoolsAsync());
    }

    private IEnumerator InitializePoolsAsync()
    {
        foreach (Pool pool in pools)
        {
            if (!pool.Prefab)
            {
                Debug.LogError($"Invalid pool configuration detected for {pool.EffectType}!");
                continue;
            }

            // Tạo container cho pool
            var container = new GameObject($"Pool_{pool.EffectType}").transform;
            container.SetParent(transform);
            pool.ContainerTransform = container;

            // Tạo object pool
            var objectPool = new Queue<GameObject>(pool.Size);
            yield return StartCoroutine(PrewarmPoolAsync(pool, objectPool));

            _poolDictionary[pool.EffectType] = objectPool;
            _poolReference[pool.EffectType] = pool;
        }
    }


    private static IEnumerator PrewarmPoolAsync(Pool pool, Queue<GameObject> objectPool)
    {
        for (int i = 0; i < pool.Size; i++)
        {
            CreateNewInstance(pool, objectPool);

            // Nhả CPU sau mỗi 10 đối tượng để tránh làm Unity treo
            if (i % 10 == 0)
            {
                yield return null;
            }
        }
    }

    private static void CreateNewInstance(Pool pool, Queue<GameObject> objectPool)
    {
        var obj = Instantiate(pool.Prefab, pool.ContainerTransform);
        obj.SetActive(false);
        obj.hideFlags = HideFlags.HideAndDontSave; // Ẩn đối tượng trong Editor
        objectPool.Enqueue(obj);
    }


    public GameObject SpawnFromPool(EffectType type, Vector3 position, Quaternion rotation)
    {
        if (!ValidatePoolExistence(type)) return null;

        var pool = _poolDictionary[type];
        var obj = pool.Count > 0 ? pool.Dequeue() : CreateNewPoolObject(type);

        SetupSpawnedObject(obj, position, rotation);
        return obj;
    }

    private bool ValidatePoolExistence(EffectType type)
    {
        if (_poolDictionary.ContainsKey(type)) return true;
        Debug.LogWarning($"Pool with type {type} doesn't exist!");
        return false;
    }

    private GameObject CreateNewPoolObject(EffectType tag)
    {
        var pool = _poolReference[tag];
        return Instantiate(pool.Prefab, pool.ContainerTransform);
    }

    private static void SetupSpawnedObject(GameObject obj, Vector3 position, Quaternion rotation)
    {
        obj.SetActive(true);
        obj.transform.SetPositionAndRotation(position, rotation);
    }

    public void ReturnToPool(EffectType type, GameObject obj)
    {
        if (!ValidatePoolExistence(type)) return;

        obj.SetActive(false);
        obj.transform.SetParent(_poolReference[type].ContainerTransform);
        _poolDictionary[type].Enqueue(obj);
    }
}