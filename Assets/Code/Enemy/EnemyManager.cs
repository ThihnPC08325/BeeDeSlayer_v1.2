using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;

    public interface IPooledObject
    {
        void OnObjectSpawn();
    }

    [System.Serializable]
    public class Pool
    {
        [SerializeField] private string _tag;
        [SerializeField] private GameObject _prefab;
        [SerializeField] private int _size;

        public string Tag => _tag;
        public GameObject Prefab => _prefab;
        public int Size => _size;
    }

    [SerializeField] private List<Pool> pools;
    private Dictionary<string, Queue<GameObject>> poolDictionary;
    [SerializeField] private Transform poolRoot; // Reference đến ---POOL---

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
        // Tìm hoặc tạo root pool
        if (poolRoot == null)
        {
            GameObject root = GameObject.Find("---POOL---");
            if (root == null)
            {
                root = new GameObject("---POOL---");
            }
            poolRoot = root.transform;
        }

        // Tạo EnemyPool container
        Transform enemyPoolContainer = poolRoot.Find("EnemyPool");
        if (enemyPoolContainer == null)
        {
            enemyPoolContainer = new GameObject("EnemyPool").transform;
            enemyPoolContainer.SetParent(poolRoot);
        }

        // Đặt EnemyManager dưới EnemyPool
        transform.SetParent(enemyPoolContainer);
    }

    private void InitializePools()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            // Tạo container cho từng loại enemy
            GameObject enemyTypeContainer = new GameObject(pool.Tag);
            enemyTypeContainer.transform.SetParent(transform);

            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.Size; i++)
            {
                GameObject obj = Instantiate(pool.Prefab, enemyTypeContainer.transform);
                obj.name = $"PooledObject{i + 1}";
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.Tag, objectPool);
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"Pool with tag {tag} doesn't exist! 🚫");
            return null;
        }

        Queue<GameObject> pool = poolDictionary[tag];
        GameObject objectToSpawn;

        if (pool.Count == 0)
        {
            // Tạo thêm enemy mới nếu pool hết
            Pool originalPool = pools.Find(p => p.Tag == tag);
            objectToSpawn = Instantiate(originalPool.Prefab);
            objectToSpawn.transform.SetParent(transform.Find(tag));
        }
        else
        {
            objectToSpawn = pool.Dequeue();
        }

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.SetPositionAndRotation(position, rotation);

        IPooledObject pooledObj = objectToSpawn.GetComponent<IPooledObject>();
        pooledObj?.OnObjectSpawn();

        return objectToSpawn;
    }
}