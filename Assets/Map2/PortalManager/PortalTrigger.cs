using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewBehaviourScript : MonoBehaviour
{
    [SerializeField] private string nextSceneName; // Tên Scene tiếp theo
    [SerializeField] private float delay = 2f;     // Thời gian chờ trước khi chuyển scene

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player đã vào Portal! Đợi " + delay + " giây trước khi chuyển Scene...");
            StartCoroutine(LoadNewSceneAfterDelay(delay));
        }
    }

    private IEnumerator LoadNewSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(nextSceneName);
    }
}
