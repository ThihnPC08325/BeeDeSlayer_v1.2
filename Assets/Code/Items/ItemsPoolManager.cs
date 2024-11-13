using System.Collections.Generic;
using UnityEngine;

public class ItemsPoolManager : MonoBehaviour
{
    public static ItemsPoolManager Instance;

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    [SerializeField] private List<Pool> pools;
    private Dictionary<string, Queue<GameObject>> poolDictionary;

    // Thêm biến để reference đến root pool
    [SerializeField] private Transform poolRoot; // Reference đến ---POOL---

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            // Kiểm tra và thiết lập cấu trúc pool
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
        // Nếu chưa assign poolRoot trong Inspector
        if (poolRoot == null)
        {
            // Tìm hoặc tạo root pool
            GameObject root = GameObject.Find("---POOL---");
            if (root == null)
            {
                root = new GameObject("---POOL---");
            }
            poolRoot = root.transform;
        }

        // Đảm bảo PoolManager là con của root pool
        transform.parent = poolRoot;
    }

    private void InitializePools()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            // Tạo một container cho mỗi loại pool
            GameObject poolContainer = new GameObject($"Pool_{pool.tag}");
            poolContainer.transform.parent = transform;

            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab, poolContainer.transform);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
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
            Pool originalPool = pools.Find(p => p.tag == tag);
            objectToSpawn = Instantiate(originalPool.prefab);
            // Thêm object mới vào đúng container
            objectToSpawn.transform.parent = transform.Find($"Pool_{tag}");
        }
        else
        {
            objectToSpawn = pool.Dequeue();
        }

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.SetPositionAndRotation(position, rotation);
        return objectToSpawn;
    }

    public void ReturnToPool(string tag, GameObject obj)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"Pool with tag {tag} doesn't exist! 🚫");
            return;
        }

        // Đảm bảo object trở về đúng container khi return
        obj.transform.parent = transform.Find($"Pool_{tag}");
        obj.SetActive(false);
        poolDictionary[tag].Enqueue(obj);
    }
}