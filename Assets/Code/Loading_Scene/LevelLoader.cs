using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Slider progressSlider;

    [Header("Loading Settings")]
    [SerializeField] private float minimumLoadingTime = 0.5f;
    [SerializeField] private float smoothSpeed = 5f;

    private static LevelLoader instance;
    public static LevelLoader Instance => instance;
    private bool isLoading;

    #region Singleton Pattern
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            ValidateReferences();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    private void ValidateReferences()
    {
        if (loadingScreen == null) Debug.LogError("Loading Screen reference is missing!");
        if (progressSlider == null) Debug.LogError("Progress Slider reference is missing!");
    }

    public void LoadLevel(int sceneIndex)
    {
        if (isLoading) return;
        if (sceneIndex < 0 || sceneIndex >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogError($"🔴 Invalid scene index: {sceneIndex}");
            return;
        }

        StartCoroutine(LoadSceneRoutine(sceneIndex));
    }

    private IEnumerator LoadSceneRoutine(int sceneIndex)
    {
        isLoading = true;
        loadingScreen.SetActive(true);

        // Set initial UI state
        progressSlider.value = 0;

        float startTime = Time.time;
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        operation.allowSceneActivation = false;

        float currentProgress = 0;

        // Loading loop
        while (currentProgress < 1 || Time.time - startTime < minimumLoadingTime)
        {
            float targetProgress = Mathf.Clamp01(operation.progress / 0.9f);
            currentProgress = Mathf.Lerp(currentProgress, targetProgress, Time.deltaTime * smoothSpeed);

            // Update UI
            progressSlider.value = currentProgress;

            if (currentProgress >= 0.99f && operation.progress >= 0.9f)
            {
                progressSlider.value = 1;
                operation.allowSceneActivation = true;
            }

            yield return null;
        }

        // Cleanup
        isLoading = false;
        loadingScreen.SetActive(false);
    }

    private void OnDestroy()
    {
        // Cleanup when destroyed
        StopAllCoroutines();
    }
}
