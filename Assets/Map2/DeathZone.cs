using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement; // Để quản lý scene
using UnityEngine.UI; // Để xử lý UI

public class DeathZone : MonoBehaviour
{
    public Image blackScreen; // Panel màu đen
    public Text deathText; // Text thông báo
    public Text deathMessage; // Reference tới Text UI
    public float delayBeforeRespawn = 1f; // Thời gian delay trước khi chuyển scene
    public PlayerController PlayerController; // Script điều khiển người chơi
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Kiểm tra nếu là Player
        {
            StartCoroutine(PlayerDeath());
        }
    }

    private IEnumerator PlayerDeath()
    {
        // Hiển thị thông báo
        deathMessage.gameObject.SetActive(true);
        deathMessage.text = "Bạn đã chết";

        //Có thể thêm: Vô hiệu hóa điều khiển người chơi
        

        // Chờ một khoảng thời gian
        yield return new WaitForSeconds(delayBeforeRespawn);

        // Chuyển sang scene map2
        SceneManager.LoadScene("Map2");
        if (PlayerController != null)
        {
            PlayerController.enabled = false;
            // Nếu có Rigidbody
            Rigidbody rb = PlayerController.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
            }
        }
        deathText.gameObject.SetActive(true);
        deathText.text = "Bạn đã chết";
    }
}