using System;
using System.Collections;
using UnityEngine;
using TMPro;

public class InteractiveText : MonoBehaviour, IInteractable
{
    [System.Serializable]
    public class UISettings
    {
        public GameObject textPanel;
        public TextMeshProUGUI descriptionText;
        [Range(0.1f, 10f)] public float fadeSpeed = 2f;
        public AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    }

    #region Inspector Fields
    [Header("Content")]
    [SerializeField] private string itemName;
    [TextArea(3, 10)]
    [SerializeField] private string itemDescription;

    [Header("Interaction Settings")]
    [SerializeField, Range(1f, 20f)] private float maxDisplayDistance = 5f;
    [SerializeField, Range(0.1f, 1f)] private float checkInterval = 0.2f;
    [SerializeField, Range(0f, 10f)] private float autoHideDelay = 0f;

    [Header("UI")]
    [SerializeField] private UISettings uiSettings;
    #endregion

    #region Events
    public event Action<bool> OnInteractionStateChanged;
    #endregion

    #region Private Variables
    private bool isDisplayingText;
    private Coroutine fadeCoroutine;
    private Coroutine distanceCheckCoroutine;
    private Transform playerTransform;
    private WaitForSeconds checkIntervalWait;
    private static readonly int MaxTextLength = 1000;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        ValidateComponents();
        checkIntervalWait = new WaitForSeconds(checkInterval);
    }

    private void Start()
    {
        InitializeUI();
        FindPlayer();
    }

    private void OnEnable() => StartDistanceCheck();
    private void OnDisable() => StopAllCoroutines();
    #endregion

    #region Public Methods
    public void Interact()
    {
        if (isDisplayingText) HideText();
        else ShowText();
    }
    #endregion

    #region Private Methods
    private void ValidateComponents()
    {
        if (uiSettings.textPanel == null)
            Debug.LogError($"Text Panel not assigned on {gameObject.name}");

        if (uiSettings.descriptionText == null)
            Debug.LogError($"Description Text component not assigned on {gameObject.name}");

        if (itemDescription.Length > MaxTextLength)
            Debug.LogWarning($"Item description on {gameObject.name} exceeds recommended length");
    }

    private void InitializeUI()
    {
        if (uiSettings.textPanel != null)
        {
            uiSettings.textPanel.SetActive(false);
            if (uiSettings.descriptionText != null)
            {
                uiSettings.descriptionText.alpha = 0;
            }
        }
    }

    private void FindPlayer()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (playerTransform == null)
            Debug.LogWarning($"Player not found for {gameObject.name}! Ensure player has 'Player' tag.");
    }

    private void StartDistanceCheck()
    {
        StopDistanceCheck();
        distanceCheckCoroutine = StartCoroutine(CheckDistanceRoutine());
    }

    private void StopDistanceCheck()
    {
        if (distanceCheckCoroutine != null)
        {
            StopCoroutine(distanceCheckCoroutine);
            distanceCheckCoroutine = null;
        }
    }

    private void ShowText()
    {
        isDisplayingText = true;
        OnInteractionStateChanged?.Invoke(true);

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(AnimateText(true));

        if (autoHideDelay > 0)
            StartCoroutine(AutoHideRoutine());
    }

    private void HideText()
    {
        isDisplayingText = false;
        OnInteractionStateChanged?.Invoke(false);

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(AnimateText(false));
    }
    #endregion

    #region Coroutines
    private IEnumerator CheckDistanceRoutine()
    {
        while (enabled)
        {
            if (playerTransform != null && isDisplayingText)
            {
                float distance = Vector3.Distance(transform.position, playerTransform.position);
                if (distance > maxDisplayDistance)
                {
                    HideText();
                }
            }
            yield return checkIntervalWait;
        }
    }

    private IEnumerator AnimateText(bool show)
    {
        float startAlpha = show ? 0 : 1;
        float targetAlpha = show ? 1 : 0;
        float elapsed = 0;

        uiSettings.textPanel.SetActive(true);
        uiSettings.descriptionText.text = itemDescription;

        while (elapsed < 1)
        {
            elapsed += Time.deltaTime * uiSettings.fadeSpeed;
            float currentAlpha = Mathf.Lerp(startAlpha, targetAlpha,
                uiSettings.fadeCurve.Evaluate(elapsed));

            uiSettings.descriptionText.alpha = currentAlpha;
            yield return null;
        }

        if (!show)
            uiSettings.textPanel.SetActive(false);
    }

    private IEnumerator AutoHideRoutine()
    {
        yield return new WaitForSeconds(autoHideDelay);
        HideText();
    }
    #endregion

    #region Gizmos
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, maxDisplayDistance);
    }
    #endregion
}