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
            GameObject container = new GameObject($"Pool_{pool.BulletType}");
            container.transform.parent = transform;
            containerDictionary[pool.BulletType] = container.transform;

            Queue<GameObject> objectPool = new Queue<GameObject>(pool.Size);
            CreateInitialPoolObjects(pool, container.transform, objectPool);
            poolDictionary[pool.BulletType] = objectPool;
        }
    }

    private void CreateInitialPoolObjects(Pool pool, Transform container, Queue<GameObject> objectPool)
    {
        for (int i = 0; i < pool.Size; i++)
        {
            GameObject obj = CreateNewPoolObject(pool.Prefab, container);
            objectPool.Enqueue(obj);
        }
    }

    private GameObject CreateNewPoolObject(GameObject prefab, Transform parent)
    {
        GameObject obj = Instantiate(prefab, parent);
        obj.SetActive(false);
        return obj;
    }

    public GameObject SpawnFromPool(BulletType bulletType, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(bulletType))
        {
            Debug.LogWarning($"Pool with type {bulletType} doesn't exist! 🚫");
            return null;
        }

        Queue<GameObject> pool = poolDictionary[bulletType];
        GameObject objectToSpawn;

        if (pool.Count == 0)
        {
            Pool originalPool = pools.Find(p => p.BulletType == bulletType);
            objectToSpawn = CreateNewPoolObject(originalPool.Prefab, containerDictionary[bulletType]);
        }
        else
        {
            objectToSpawn = pool.Dequeue();
        }

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.SetPositionAndRotation(position, rotation);
        return objectToSpawn;
    }

    public void ReturnToPool(BulletType bulletType, GameObject obj)
    {
        if (!poolDictionary.ContainsKey(bulletType))
        {
            Debug.LogWarning($"Pool with type {bulletType} doesn't exist! 🚫");
            return;
        }

        obj.transform.parent = containerDictionary[bulletType];
        obj.SetActive(false);
        poolDictionary[bulletType].Enqueue(obj);
    }
}