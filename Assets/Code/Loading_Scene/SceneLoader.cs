using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            LevelLoader.Instance.LoadLevel(GetNextSceneIndex());
        }
    }

    private static int GetNextSceneIndex()
    {
        return SceneManager.GetActiveScene().buildIndex + 1;
    }
}