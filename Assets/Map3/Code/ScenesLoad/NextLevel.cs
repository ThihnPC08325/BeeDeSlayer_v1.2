using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NextLevel : MonoBehaviour
{
    [SerializeField] private GameObject loaderUI;
    private void Start()
    {
        loaderUI.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Load scene tiếp theo
            LoadScene();
            loaderUI.SetActive(true);
        }
    }
    [SerializeField] private Slider proressSlider;
    [SerializeField] private int sceneIndex;
    public void LoadScene()
    {
        StartCoroutine(SceneLoad(SceneManager.GetActiveScene().buildIndex +1));
    }
    private IEnumerator SceneLoad(int sceneIndex)
    {
        proressSlider.value = 0;
        loaderUI.SetActive(true);

        AsyncOperation asyncoperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneIndex);
        asyncoperation.allowSceneActivation = false;

        float progress = 0;
        while (!asyncoperation.isDone)
        {
            progress = Mathf.MoveTowards(progress, asyncoperation.progress, Time.deltaTime);
            proressSlider.value = progress;
            if (progress >= 0.9f)
            {
                proressSlider.value = 1;
                asyncoperation.allowSceneActivation = true;
            }
            yield return null;
        }
    }
}
