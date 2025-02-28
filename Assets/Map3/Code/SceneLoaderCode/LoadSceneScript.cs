using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadSceneScript : MonoBehaviour
{
    [SerializeField] private GameObject loaderUI;
    [SerializeField] private Slider progressSlider;


    public void LoadScene(int index)
    {
        StartCoroutine(Loadscene_Couroutine(index));
    }
    private IEnumerator Loadscene_Couroutine(int index)
    {
        progressSlider.value = 0;
        loaderUI.SetActive(true);
        AsyncOperation operation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(index);

        float progress = 0;
        while (!operation.isDone)
        {
            progress = Mathf.Clamp01(operation.progress / 0.9f);
            progressSlider.value = progress;
            yield return null;
        }
    }
}
