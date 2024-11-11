using UnityEngine;

public interface IInteractable
{
    void Interact();
}

[RequireComponent(typeof(Collider))] // Đảm bảo có Collider
public class Interactor : MonoBehaviour
{
    #region Inspector Fields
    [Header("Interaction Settings")]
    [SerializeField] private Transform interactorSource; //Camera người chơi
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    [Header("UI")]
    [SerializeField] private GameObject textPanel;
    [SerializeField] private TMPro.TextMeshProUGUI promptText;
    [SerializeField] private string promptMessage;
    #endregion

    #region Private Variables
    private bool isInteracting;
    private InteractiveText currentInteractiveText;
    private Camera mainCamera;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        ValidateComponents();
        mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        if (currentInteractiveText != null)
            currentInteractiveText.OnInteractionStateChanged += OnInteractionStateChanged;
    }

    private void OnDisable()
    {
        UnsubscribeFromCurrentText();
    }

    private void Update()
    {
        CheckForInteractable();

        if (Input.GetKeyDown(interactKey))
            TryInteract();
    }
    #endregion

    #region Private Methods
    private void ValidateComponents()
    {
        if (interactorSource == null)
        {
            interactorSource = transform;
            Debug.LogWarning($"[{gameObject.name}] InteractorSource not set, using this transform.");
        }

        if (textPanel == null || promptText == null)
            Debug.LogError($"[{gameObject.name}] UI components not fully assigned!");
    }

    private void CheckForInteractable()
    {
        if (isInteracting)
        {
            HidePrompt();
            return;
        }

        Ray ray = new(interactorSource.position, interactorSource.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
        {
            if (hit.collider.TryGetComponent(out InteractiveText _))
                ShowPrompt();
            else
                HidePrompt();
        }
        else
            HidePrompt();
    }

    private void TryInteract()
    {
        Ray ray = new(interactorSource.position, interactorSource.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
        {
            if (hit.collider.TryGetComponent(out InteractiveText interactiveText))
            {
                HandleInteraction(interactiveText);
            }
        }
    }

    private void HandleInteraction(InteractiveText newInteractiveText)
    {
        if (currentInteractiveText != newInteractiveText)
        {
            UnsubscribeFromCurrentText();
            currentInteractiveText = newInteractiveText;
            currentInteractiveText.OnInteractionStateChanged += OnInteractionStateChanged;
        }

        currentInteractiveText.Interact();
    }

    private void ShowPrompt()
    {
        if (textPanel != null && promptText != null)
        {
            textPanel.SetActive(true);
            promptText.text = promptMessage;
        }
    }

    private void HidePrompt()
    {
        if (textPanel != null)
            textPanel.SetActive(false);
    }

    private void OnInteractionStateChanged(bool isDisplaying)
    {
        isInteracting = isDisplaying;
        if (isDisplaying)
            HidePrompt();
    }

    private void UnsubscribeFromCurrentText()
    {
        if (currentInteractiveText != null)
        {
            currentInteractiveText.OnInteractionStateChanged -= OnInteractionStateChanged;
            currentInteractiveText = null;
        }
    }
    #endregion
}