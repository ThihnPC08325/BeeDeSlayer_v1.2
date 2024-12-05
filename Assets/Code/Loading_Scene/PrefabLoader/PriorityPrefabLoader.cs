using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriorityPrefabLoader : MonoBehaviour
{
    [System.Serializable]
    public class PrefabItem
    {
        [Tooltip("Prefab cần tải")]
        [SerializeField] private GameObject prefab; // Prefab để tải
        [Tooltip("Độ ưu tiên (thấp hơn là ưu tiên hơn)")]
        [SerializeField] private int priority;     // Độ ưu tiên (thấp hơn là ưu tiên hơn)
        [Tooltip("Muốn phân loại prefab này theo Gameobject nào? (Nếu để trống, script sẽ tự động tạo một container.)")]
        [SerializeField] private Transform customParent; // Transform riêng cho từng prefab

        public GameObject Prefab => prefab;
        public int Priority => priority;
        public Transform CustomParent => customParent;
    }

    [Header("Prefabs to Load")]
    [SerializeField] private List<PrefabItem> prefabsToLoad;

    [Header("Loading Settings")]
    [Tooltip("Thời gian chờ giữa các lần tải prefab")]
    [SerializeField] private float delayBetweenLoads = 0.1f; // Thời gian chờ giữa các lần tải

    private bool isLoading = false;
    private Dictionary<GameObject, Transform> prefabInstances = new Dictionary<GameObject, Transform>();

    public IEnumerator LoadPrefabsAsync()
    {
        if (isLoading)
        {
            Debug.LogWarning("Prefab loading is already in progress!");
            yield break;
        }

        isLoading = true;

        // Sắp xếp prefab theo thứ tự ưu tiên
        prefabsToLoad.Sort((a, b) => a.Priority.CompareTo(b.Priority));

        foreach (PrefabItem item in prefabsToLoad)
        {
            // Kiểm tra prefab hợp lệ
            if (item.Prefab == null)
            {
                Debug.LogWarning("Prefab is missing, skipping...");
                continue;
            }

            // Xác định parent (dùng custom parent nếu có, ngược lại tạo mới)
            Transform parent = item.CustomParent ? item.CustomParent : new GameObject($"{item.Prefab.name}_Container").transform;

            // Instantiate prefab và thêm vào dictionary
            GameObject instance = Instantiate(item.Prefab, parent);
            instance.SetActive(false); // Tắt đối tượng để không gây tải thêm
            prefabInstances.Add(instance, parent);

            // Đợi trước khi tải prefab tiếp theo
            yield return new WaitForSeconds(delayBetweenLoads);
        }

        Debug.Log("All prefabs loaded!");
        isLoading = false;
    }

    // Gọi để kích hoạt tất cả prefab
    public void ActivateAllPrefabs()
    {
        foreach (var instance in prefabInstances.Keys)
        {
            instance.SetActive(true);
        }
    }

    // Gọi để kích hoạt các prefab thuộc một parent cụ thể
    public void ActivatePrefabsUnderParent(Transform parent)
    {
        foreach (var kvp in prefabInstances)
        {
            if (kvp.Value == parent)
            {
                kvp.Key.SetActive(true);
            }
        }
    }

    // Gọi để xóa tất cả prefab thuộc một parent cụ thể
    public void DestroyPrefabsUnderParent(Transform parent)
    {
        List<GameObject> toDestroy = new();

        foreach (var kvp in prefabInstances)
        {
            if (kvp.Value == parent)
            {
                toDestroy.Add(kvp.Key);
            }
        }

        foreach (var obj in toDestroy)
        {
            prefabInstances.Remove(obj);
            Destroy(obj);
        }
    }
}