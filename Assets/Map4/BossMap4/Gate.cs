using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Gate : MonoBehaviour
{
    [SerializeField] private Image fadeImage; // UI Image dùng để tối dần
    [SerializeField] private float fadeDuration = 3f; // Thời gian tối màn hình
    [SerializeField] private string nextScene = "EndGame"; // Tên scene chuyển đến

    private void OnTriggerEnter(Collider other)
    {
        // Kiểm tra nếu người chơi chạm vào cổng
        if (other.CompareTag("Player"))
        {
            StartCoroutine(TransitionToEndGame());
        }
    }

    private IEnumerator TransitionToEndGame()
    {
        // Tối màn hình dần dần
        float elapsed = 0f;
        Color startColor = fadeImage.color;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            fadeImage.color = new Color(startColor.r, startColor.g, startColor.b, Mathf.Lerp(0f, 1f, elapsed / fadeDuration));
            yield return null;
        }

        // Sau khi tối màn hình, chuyển scene
        SceneManager.LoadScene(nextScene);
    }
}
