using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Kiểm tra nếu Player chạm vào
        {
            SceneManager.LoadScene("Map2"); // Thay "NextScene" bằng tên scene tiếp theo
        }
    }
}
