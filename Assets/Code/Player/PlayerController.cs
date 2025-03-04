using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    #region SerializeField
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 12f; // Tăng tốc độ di chuyển
    [SerializeField] private float maxSpeed = 20f;
    [SerializeField] private float currentSpeedModifier = 1f;
    [SerializeField] private float acceleration = 150f;
    [SerializeField] private float deceleration = 200f;
    [SerializeField] private float airControl = 0.8f; // Kiểm soát khi ở trên không

    [Header("Jumping")]
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private int maxJumps = 2;
    [SerializeField] private float gravity = -25f; // Tăng trọng lực để nhảy nhanh hơn

    [Header("Dashing")]
    [SerializeField] private float dashSpeed = 30f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private int maxDashes = 2;
    
    [Header("Animation")]
    [SerializeField] private Animator arGunReload;
    [SerializeField] private Animator kbGunReload;
    #endregion

    #region private
    private float _originalSpeed;
    private CharacterController _controller;
    private Vector2 _moveInput;
    private Vector3 _velocity;
    private bool _isGrounded;
    private int _jumpsRemaining;
    private bool _isDashing;
    private int _remainingDashes;
    private float _dashEndTime;

    private PlayerInput _playerInput;
    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _dashAction;
    private InputAction _reload;
    #endregion

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _playerInput = new PlayerInput();
        _originalSpeed = moveSpeed;
    }

    private void OnEnable()
    {
        _moveAction = _playerInput.Player.Move;
        _moveAction.Enable();

        _jumpAction = _playerInput.Player.Jump;
        _jumpAction.Enable();

        _dashAction = _playerInput.Player.Dash;
        _dashAction.Enable();

        _reload = _playerInput.Player.Reload;
        _reload.Enable();
    }

    private void OnDisable()
    {
        _moveAction.Disable();
        _jumpAction.Disable();
        _dashAction.Disable();
    }

    private void FixedUpdate()
    {
        _isGrounded = _controller.isGrounded;
        if (!_isGrounded || !(_velocity.y < 0)) return;
        _velocity.y = -2f;
        _jumpsRemaining = maxJumps;
        _remainingDashes = maxDashes;
    }

    private void Update()
    {
        HandleMovement();
        ApplyGravity();
        if (!_dashAction.triggered || _remainingDashes <= 0) return;
        StartCoroutine(Dash());
        _remainingDashes--;
    }
    public void ApplySlow(float slowPercentage)
    {
        moveSpeed = _originalSpeed * (1 - slowPercentage);
    }

    public void RemoveSlow()
    {
        moveSpeed = _originalSpeed;
    }
    private void HandleMovement()
    {
        _moveInput = _moveAction.ReadValue<Vector2>();
        Vector3 move = transform.right * _moveInput.x + transform.forward * _moveInput.y;

        if (_isDashing)
        {
            if (Time.time >= _dashEndTime)
            {
                _isDashing = false;
            }
            else
            {
                _controller.Move(move.normalized * (dashSpeed * Time.deltaTime));
                return;
            }
        }

        //---MOVE---
        float targetSpeed = move.magnitude * moveSpeed * currentSpeedModifier;
        float currentSpeed = new Vector3(_controller.velocity.x, 0, _controller.velocity.z).magnitude;

        currentSpeed = targetSpeed > currentSpeed ? Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.deltaTime) : Mathf.MoveTowards(currentSpeed, targetSpeed, deceleration * Time.deltaTime);

        currentSpeed = Mathf.Clamp(currentSpeed, 0, maxSpeed);

        if (move.magnitude > 0.1f)
        {
            _controller.Move(move.normalized * (currentSpeed * Time.deltaTime));
        }

        if (_isGrounded)
        {
            _controller.Move(move * (moveSpeed * Time.deltaTime));
        }
        else
        {
            //---AIR CONTROL---
            _controller.Move(move * (moveSpeed * airControl * Time.deltaTime));
        }

        //---JUMP---
        if (!_jumpAction.triggered || _jumpsRemaining <= 0) return;
        _velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        _jumpsRemaining--;
    }

    private void ApplyGravity()
    {
        _velocity.y += gravity * Time.deltaTime;
        _controller.Move(_velocity * Time.deltaTime);
    }

    private IEnumerator Dash()
    {
        _isDashing = true;
        _dashEndTime = Time.time + dashDuration;

        // Đọc input hiện tại để xác định hướng dash
        Vector2 input = _moveAction.ReadValue<Vector2>();

        // Dash theo hướng input
        Vector3 dashDirection = transform.TransformDirection(new Vector3(input.x, 0, input.y)).normalized;

        // Tính toán vận tốc dash
        _velocity = dashDirection * dashSpeed;

        // Thực hiện dash
        while (Time.time < _dashEndTime)
        {
            _controller.Move(_velocity * Time.deltaTime);
            yield return null;
        }

        // Kết thúc dash
        _isDashing = false;
        _velocity = Vector3.zero;
    }

    public void ApplySpeedModifier(float modifier)
    {
        currentSpeedModifier = modifier;
    }

    public void RemoveSpeedModifier()
    {
        currentSpeedModifier = 1f;
    }
    private void OnReload() 
    {
        arGunReload.Play("ArGunReload");
        kbGunReload.Play("KBGunReload");
    }
    private IEnumerator SlowDown(float multiplier, float duration)
    {
        currentSpeedModifier = multiplier; // Giảm tốc độ bằng modifier
        Debug.Log("Speed reduced to: " + (moveSpeed * currentSpeedModifier)); // Debug kiểm tra

        yield return new WaitForSeconds(duration); // Chờ hết thời gian làm chậm

        currentSpeedModifier = 1f; // Khôi phục tốc độ bình thường
        Debug.Log("Speed restored to: " + (moveSpeed * currentSpeedModifier)); // Debug kiểm tra
    }

    public void ModifySpeed(float multiplier, float duration)
    {
        StartCoroutine(SlowDown(multiplier, duration));
    }
}