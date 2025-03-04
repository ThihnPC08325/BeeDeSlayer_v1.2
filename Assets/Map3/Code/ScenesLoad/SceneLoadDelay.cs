using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoadDelay : MonoBehaviour
{
    [SerializeField] private GameObject loaderUI;
    [SerializeField] private Slider progressSlider;
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
        progressSlider.value = 0;
        loaderUI.SetActive(true);

        AsyncOperation asyncoperation = SceneManager.LoadSceneAsync(sceneIndex);
        if (asyncoperation == null) yield break;
        asyncoperation.allowSceneActivation = false;

        float progress = 0;
        while (!asyncoperation.isDone)
        {
            progress = Mathf.MoveTowards(progress, asyncoperation.progress, Time.deltaTime);
            progressSlider.value = progress;
            if (progress >= 0.9f)
            {
                progressSlider.value = 1;
                asyncoperation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
