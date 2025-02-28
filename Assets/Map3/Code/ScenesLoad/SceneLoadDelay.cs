using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoadDelay : MonoBehaviour
{
    [SerializeField] private GameObject loaderUI;
    [SerializeField] private Slider proressSlider;
    [SerializeField] private int sceneIndex;
    private void Start()
    {
        StartCoroutine(SceneLoad(SceneManager.GetActiveScene().buildIndex +1));
    }
    public void LoadScene()
    {
        StartCoroutine(SceneLoad(SceneManager.GetActiveScene().buildIndex + 1));
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
