using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SceneChanger : MonoBehaviour
{
    private static readonly System.Random Random = new System.Random();
    private readonly List<string> _scenes = new List<string> { "Man1.1", "Man1.2", "Man1.3", "Man1.4" };
    private const string TargetScene = "Man1.5";

    [SerializeField] private NoteCounter noteCounter;

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void LoadRandomScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        List<string> availableScenes = new List<string>(_scenes);
        availableScenes.Remove(currentSceneName);
        string nextScene = availableScenes[Random.Next(availableScenes.Count)];
        Debug.Log($"Loading random scene: {nextScene}");
        SceneManager.LoadScene(nextScene);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        Debug.Log($"Player triggered scene change. Notes collected: {noteCounter?.CollectedNoteCount}");
        if (noteCounter != null && noteCounter.CollectedNoteCount >= 4)
        {
            Debug.Log("Waiting for correct password...");
            // Không load scene ngay, chờ nhập mật khẩu từ GhostProfessor
        }
        else
        {
            LoadRandomScene();
        }
    }

    public void LoadTargetScene()
    {
        Debug.Log($"Loading target scene: {TargetScene}");
        SceneManager.LoadScene(TargetScene);
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Scene loaded: {scene.name}");
        DynamicGI.UpdateEnvironment();
    }
}
