using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement; // Để quản lý scene
using UnityEngine.UI; // Để xử lý UI

public class DeathZone : MonoBehaviour
{
    public Text deathMessage; // Reference tới Text UI
    public float delayBeforeRespawn = 1f; // Thời gian delay trước khi chuyển scene

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
        // player.GetComponent<PlayerController>().enabled = false;

        // Chờ một khoảng thời gian
        yield return new WaitForSeconds(delayBeforeRespawn);

        // Chuyển sang scene map2
        SceneManager.LoadScene("Map2");
    }
}