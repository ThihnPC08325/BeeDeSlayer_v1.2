using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization; // Để quản lý scene
using UnityEngine.UI; // Để xử lý UI

public class DeathZone : MonoBehaviour
{
    [SerializeField] private Image blackScreen; // Panel màu đen
    [SerializeField] private Text deathText; // Text thông báo
    [SerializeField] private Text deathMessage; // Reference tới Text UI
    [SerializeField] private float delayBeforeRespawn = 1f; // Thời gian delay trước khi chuyển scene
    [SerializeField] private PlayerController playerController; // Script điều khiển người chơi

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
        if (playerController)
        {
            playerController.enabled = false;
            // Nếu có Rigidbody
            Rigidbody rb = playerController.GetComponent<Rigidbody>();
            if (rb)
            {
                rb.isKinematic = true;
            }
        }

        deathText.gameObject.SetActive(true);
        deathText.text = "Bạn đã chết";
    }
}