using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Basic Setting")]
    [SerializeField] private float baseMultiplier = 1f;
    [SerializeField] private float sensitivityX = 2f;
    [SerializeField] private float sensitivityY = 2f;
    [SerializeField] private bool useMouseAcceleration = false;
    [SerializeField] private bool invertY = false;
    [SerializeField] private bool invertX = false; // New: Invert horizontal input

    [Header("AnimationCurve")]
    [SerializeField] private AnimationCurve sensitivityCurve = AnimationCurve.Linear(0, 1, 1, 1);
    [SerializeField] private AnimationCurve adsCurve = AnimationCurve.Linear(0, 1, 1, 0.5f);
    [SerializeField] private AnimationCurve accelerationCurve = AnimationCurve.EaseInOut(0, 1, 1, 2);

    [SerializeField] Transform playerBody;

    #region private
    private Vector2 _currentRotation;
    private PlayerInput _playerInput;
    private InputAction _lookAction;
    private const bool IsAiming = false;
    private bool _isConfused = false; // New: Confusion status
    #endregion

    private void Awake()
    {
        _playerInput = new PlayerInput();
        _lookAction = _playerInput.Player.Look;
    }

    private void OnEnable()
    {
        SetCameraControlEnabled(true);
    }

    private void OnDisable()
    {
        SetCameraControlEnabled(false);
    }

    private void Update()
    {
        HandleMouseLook();
    }

    private void HandleMouseLook()
    {
        Vector2 lookInput = _lookAction.ReadValue<Vector2>();
        float inputMagnitude = lookInput.magnitude;

        // Apply sensitivity curve
        float sensitivityMultiplier = sensitivityCurve.Evaluate(inputMagnitude);

        // Apply ADS (Aim Down Sight) sensitivity
        float adsMultiplier = IsAiming ? adsCurve.Evaluate(inputMagnitude) : 1f;

        // Apply acceleration
        float accelerationMultiplier = useMouseAcceleration ? accelerationCurve.Evaluate(inputMagnitude) : 1f;

        // Final sensitivity calculation
        float finalMultiplier = baseMultiplier * sensitivityMultiplier * adsMultiplier * accelerationMultiplier;

        // Process input with sensitivity
        Vector2 processedInput = new Vector2(
            lookInput.x * sensitivityX * finalMultiplier,
            lookInput.y * sensitivityY * finalMultiplier
        ) * Time.deltaTime;

        // Apply inversion
        if (invertX) processedInput.x = -processedInput.x;
        if (invertY) processedInput.y = -processedInput.y;

        // Apply confusion effect
        if (_isConfused)
        {
            // Swap X and Y input
            (processedInput.x, processedInput.y) = (processedInput.y, processedInput.x);
        }

        // Update rotation
        _currentRotation.x = Mathf.Clamp(_currentRotation.x - processedInput.y, -90f, 90f);
        _currentRotation.y += processedInput.x;

        // Apply rotation
        transform.localRotation = Quaternion.Euler(_currentRotation.x, 0f, 0f);
        playerBody.localRotation = Quaternion.Euler(0f, _currentRotation.y, 0f);
    }

    public void SetCameraControlEnabled(bool enabled)
    {
        if (enabled)
        {
            _lookAction.Enable();
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            _lookAction.Disable();
            Cursor.lockState = CursorLockMode.None;
        }
    }

    // Apply confusion for a set duration
    public void ApplyConfusion(float duration)
    {
        if (_isConfused) return;
        _isConfused = true;
        Debug.Log("Camera Confusion Activated! X and Y swapped.");
        Invoke(nameof(RemoveConfusion), duration);
    }

    private void RemoveConfusion()
    {
        _isConfused = false;
        Debug.Log("Camera Confusion Wore Off! Controls restored.");
    }
}
