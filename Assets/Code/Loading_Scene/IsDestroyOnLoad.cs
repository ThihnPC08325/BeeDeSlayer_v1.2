using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IsDestroyOnLoad : MonoBehaviour
{
    [Tooltip("Nếu true, GameObject này sẽ bị hủy khi chuyển scene. Nếu false, nó sẽ được giữ lại.")]
    [SerializeField] private bool destroyOnSceneChange = true;

    [Tooltip("Danh sách scene names mà object sẽ bị destroy (Hoạt động khi destroyOnSceneChange = true)")]
    [SerializeField] private string[] scenesToDestroyOn;

    [Tooltip("Thời gian delay trước khi destroy (giây)")]
    [SerializeField] private float destroyDelay = 0f;

    [Header("Child Object Management")]
    [Tooltip("Xử lý các object con khi parent bị destroy")]
    [SerializeField] private ChildHandling childHandlingMode = ChildHandling.DestroyWithParent;

    [Tooltip("Danh sách các tag của child objects sẽ được giữ lại")]
    [SerializeField] private string[] childTagsToPreserve;

    [Tooltip("Danh sách các child objects cụ thể sẽ được giữ lại")]
    [SerializeField] private GameObject[] childrenToPreserve;

    [Header("Debug Settings")]
    [SerializeField] private bool showDebugLogs = false;

    private static int instanceCount = 0;
    private int instanceId;
    private List<GameObject> preservedChildren = new List<GameObject>();

    // Enum định nghĩa cách xử lý các object con
    public enum ChildHandling
    {
        DestroyWithParent,      // Xóa tất cả con
        PreserveAll,            // Giữ lại tất cả con
        PreserveSelected,       // Chỉ giữ lại các con được chọn
        PreserveByTag           // Giữ lại các con có tag được chỉ định
    }

    private void Awake()
    {
        instanceId = ++instanceCount;

        if (!destroyOnSceneChange)
        {
            try
            {
                DontDestroyOnLoad(gameObject);
                LogDebug($"Object {gameObject.name} (ID: {instanceId}) marked as DontDestroyOnLoad");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to set DontDestroyOnLoad: {e.Message}");
            }
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        LogDebug($"Object {gameObject.name} (ID: {instanceId}) registered scene load callback");
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        LogDebug($"Object {gameObject.name} (ID: {instanceId}) unregistered scene load callback");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!destroyOnSceneChange) return;

        if (scenesToDestroyOn != null && scenesToDestroyOn.Length > 0)
        {
            bool shouldDestroy = System.Array.Exists(scenesToDestroyOn,
                                                   sceneName => sceneName == scene.name);
            if (!shouldDestroy) return;
        }

        if (destroyDelay > 0)
        {
            StartCoroutine(DestroyWithDelay());
        }
        else
        {
            DestroyObject();
        }
    }

    private System.Collections.IEnumerator DestroyWithDelay()
    {
        LogDebug($"Object {gameObject.name} (ID: {instanceId}) will be destroyed in {destroyDelay} seconds");
        yield return new WaitForSeconds(destroyDelay);
        DestroyObject();
    }

    private void DestroyObject()
    {
        HandleChildren();
        LogDebug($"Destroying object {gameObject.name} (ID: {instanceId})");
        Destroy(gameObject);
    }

    private void HandleChildren()
    {
        Transform[] children = GetComponentsInChildren<Transform>();

        switch (childHandlingMode)
        {
            case ChildHandling.PreserveAll:
                PreserveAllChildren(children);
                break;

            case ChildHandling.PreserveSelected:
                PreserveSelectedChildren();
                break;

            case ChildHandling.PreserveByTag:
                PreserveChildrenByTag(children);
                break;

            case ChildHandling.DestroyWithParent:
            default:
                LogDebug($"All children will be destroyed with parent {gameObject.name}");
                break;
        }
    }

    private void PreserveAllChildren(Transform[] children)
    {
        foreach (Transform child in children)
        {
            if (child != transform) // Không xử lý chính nó
            {
                PreserveChild(child.gameObject);
            }
        }
    }

    private void PreserveSelectedChildren()
    {
        if (childrenToPreserve != null)
        {
            foreach (GameObject child in childrenToPreserve)
            {
                if (child != null)
                {
                    PreserveChild(child);
                }
            }
        }
    }

    private void PreserveChildrenByTag(Transform[] children)
    {
        if (childTagsToPreserve != null)
        {
            foreach (Transform child in children)
            {
                if (child != transform && // Không xử lý chính nó
                    System.Array.Exists(childTagsToPreserve, tag => child.CompareTag(tag)))
                {
                    PreserveChild(child.gameObject);
                }
            }
        }
    }

    private void PreserveChild(GameObject child)
    {
        child.transform.SetParent(null);
        if (!destroyOnSceneChange)
        {
            DontDestroyOnLoad(child);
        }
        preservedChildren.Add(child);
        LogDebug($"Child object {child.name} preserved");
    }

    private void LogDebug(string message)
    {
        if (showDebugLogs)
        {
            Debug.Log($"[DestroyOnLoad] {message}");
        }
    }

    private void OnValidate()
    {
        if (destroyDelay < 0)
        {
            destroyDelay = 0;
            Debug.LogWarning("Destroy delay cannot be negative. Setting to 0.");
        }
    }
}
