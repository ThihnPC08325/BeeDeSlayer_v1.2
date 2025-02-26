using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SceneChanger : MonoBehaviour
{
    private static readonly System.Random _random = new System.Random();
    private readonly List<string> _scenes = new List<string> { "Man1.1", "Man1.2", "Man1.3", "Man1.4"};
    private const string TargetScene = "Man1.5";

    [SerializeField] private NoteCounter noteCounter; // Reference to NoteCounter

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
        string nextScene = availableScenes[_random.Next(availableScenes.Count)];
        Debug.Log($"Loading random scene: {nextScene}");
        SceneManager.LoadScene(nextScene);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"Player triggered scene change. Notes collected: {noteCounter?.CollectedNoteCount}");
            if (noteCounter != null && noteCounter.CollectedNoteCount >= 4)
            {
                Debug.Log($"Loading target scene: {TargetScene}");
                SceneManager.LoadScene(TargetScene);
            }
            else
            {
                LoadRandomScene();
            }
        }
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Scene loaded: {scene.name}");
        DynamicGI.UpdateEnvironment();
        // Nếu có logic nào khác liên quan đến việc load scene, có thể giữ lại ở đây
    }
}
