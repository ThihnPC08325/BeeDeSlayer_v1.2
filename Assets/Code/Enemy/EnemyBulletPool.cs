using System.Collections.Generic;
using UnityEngine;

public class EnemyBulletPool : MonoBehaviour
{
    public enum BulletType
    {
        Normal,
        Fire,
        Ice,
        Poison,

        //Skill Boss map2
        SkillBossMap2_Eyes,
        SkillBossMap2_Table,
        SkillBossMap2_DeadBeam,
        // Thêm các loại đạn khác nếu cần
    }

    [System.Serializable]
    public class Pool
    {
        [SerializeField] private BulletType _bulletType;
        [SerializeField] private GameObject _prefab;
        [SerializeField] private int _size;

        public BulletType BulletType => _bulletType;
        public GameObject Prefab => _prefab;
        public int Size => _size;
    }

    private static EnemyBulletPool instance;
    public static EnemyBulletPool Instance => instance;

    [SerializeField] private List<Pool> pools;
    [SerializeField] private Transform poolRoot;

    private Dictionary<BulletType, Queue<GameObject>> poolDictionary;
    private Dictionary<BulletType, Transform> containerDictionary;

    private const string POOL_ROOT_NAME = "---POOL---";

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
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
        poolRoot ??= GameObject.Find(POOL_ROOT_NAME)?.transform
                  ?? new GameObject(POOL_ROOT_NAME).transform;
        transform.parent = poolRoot;
    }

    private void InitializePools()
    {
        poolDictionary = new Dictionary<BulletType, Queue<GameObject>>();
        containerDictionary = new Dictionary<BulletType, Transform>();

        foreach (Pool pool in pools)
        {
            // Tạo container cho mỗi loại đạn
            Transform container = CreateContainer(pool.BulletType);
            containerDictionary[pool.BulletType] = container;

            // Tạo pool ban đầu
            Queue<GameObject> objectPool = new Queue<GameObject>(pool.Size);
            for (int i = 0; i < pool.Size; i++)
            {
                objectPool.Enqueue(CreateInactiveObject(pool.Prefab, container));
            }

            poolDictionary[pool.BulletType] = objectPool;
        }
    }

    private Transform CreateContainer(BulletType bulletType)
    {
        GameObject container = new GameObject($"Pool_{bulletType}");
        container.transform.parent = poolRoot;
        return container.transform;
    }

    private GameObject CreateInactiveObject(GameObject prefab, Transform parent)
    {
        GameObject obj = Instantiate(prefab, parent);
        obj.SetActive(false);
        return obj;
    }

    public GameObject SpawnFromPool(BulletType bulletType, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.TryGetValue(bulletType, out var pool))
        {
            Debug.LogWarning($"Pool {bulletType} not found!");
            return null;
        }

        GameObject objectToSpawn = pool.Count > 0
            ? pool.Dequeue()
            : CreateInactiveObject(pools.Find(p => p.BulletType == bulletType).Prefab, containerDictionary[bulletType]);

        objectToSpawn.transform.SetPositionAndRotation(position, rotation);
        objectToSpawn.SetActive(true);
        return objectToSpawn;
    }

    public void ReturnToPool(BulletType bulletType, GameObject obj)
    {
        if (poolDictionary.TryGetValue(bulletType, out var pool))
        {
            obj.SetActive(false);
            obj.transform.parent = containerDictionary[bulletType];
            pool.Enqueue(obj);
        }
        else
        {
            Debug.LogWarning($"Attempted to return object to nonexistent pool {bulletType}.");
        }
    }
}