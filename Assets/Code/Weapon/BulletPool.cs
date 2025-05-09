﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class BulletPool : MonoBehaviour
{
    private static BulletPool s_Instance;
    public static BulletPool Instance
    {
        get
        {
            if (s_Instance) return s_Instance;
            // Tìm trong scene
            s_Instance = FindObjectOfType<BulletPool>();
            if (s_Instance) return s_Instance;
            // Nếu không tìm thấy, tạo mới
            var obj = new GameObject("PlayerBulletPool");
            s_Instance = obj.AddComponent<BulletPool>();
            return s_Instance;
        }
    }

    public enum PoolType
    {
        NormalBullet,
        ArmorPiercingBullet,
        // Thêm các loại khác
    }

    [System.Serializable]
    public class Pool
    {
        [SerializeField] private PoolType type;
        [SerializeField] private GameObject prefab;
        [SerializeField] private int size;

        public PoolType Type => type;
        public GameObject Prefab => prefab;
        public int Size => size;
    }

    [SerializeField] private List<Pool> pools;
    [SerializeField] private Transform poolRoot;

    // Cache các components
    private readonly Dictionary<PoolType, Queue<GameObject>> _poolDictionary = new();
    private readonly Dictionary<PoolType, Transform> _poolContainers = new();

    private void Awake()
    {
        if (s_Instance == null)
        {
            s_Instance = this;
            SetupPoolStructure();
            DontDestroyOnLoad(poolRoot.gameObject);
            StartCoroutine(InitializePoolsAsync()); // Sử dụng Coroutine
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void SetupPoolStructure()
    {
        if (poolRoot == null)
        {
            var root = GameObject.Find("---POOL---");
            poolRoot = root != null ? root.transform : new GameObject("---POOL---").transform;
        }
        transform.parent = poolRoot;
    }

    private IEnumerator InitializePoolsAsync()
    {
        foreach (var pool in pools)
        {
            var container = new GameObject($"Pool_{pool.Type}").transform;
            container.parent = transform;
            _poolContainers[pool.Type] = container;

            var objectPool = new Queue<GameObject>(pool.Size);

            for (int i = 0; i < pool.Size; i++)
            {
                CreateNewInstance(pool, container, objectPool);
                if (i % 10 == 0) yield return null; // Tạo 10 object mỗi frame
            }

            _poolDictionary[pool.Type] = objectPool;
        }
    }

    private static void CreateNewInstance(Pool pool, Transform container, Queue<GameObject> objectPool)
    {
        var obj = Instantiate(pool.Prefab, container);
        obj.SetActive(false);
        objectPool.Enqueue(obj);
    }

    public GameObject SpawnFromPool(PoolType type, Vector3 position, Quaternion rotation)
    {
        if (!_poolDictionary.TryGetValue(type, out Queue<GameObject> pool))
        {
            Debug.LogWarning($"Pool of type {type} doesn't exist!");
            return null;
        }

        GameObject obj;
        if (pool.Count == 0)
        {
            var originalPool = pools.Find(p => p.Type == type);
            obj = Instantiate(originalPool.Prefab, _poolContainers[type]);
        }
        else
        {
            obj = pool.Dequeue();
        }

        obj.SetActive(true);
        obj.transform.SetPositionAndRotation(position, rotation);

        return obj;
    }

    public void ReturnToPool(PoolType type, GameObject obj)
    {
        if (!_poolDictionary.ContainsKey(type))
        {
            Debug.LogWarning($"Pool of type {type} doesn't exist!");
            return;
        }

        obj.transform.parent = _poolContainers[type];
        obj.SetActive(false);
        _poolDictionary[type].Enqueue(obj);
    }
}