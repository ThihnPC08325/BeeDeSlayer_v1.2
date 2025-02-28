using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Kiểm tra nếu Player chạm vào
        {
            LoadNextSceneByBuildIndex();
        }
    }

    private static void LoadNextSceneByBuildIndex()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentIndex + 1); // Chuyển sang scene kế tiếp
    }
}