using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

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
    [SerializeField] private Animator ArGunReload;
    [SerializeField] private Animator KBGunReload;
    #endregion

    #region private
    private float originalSpeed;
    private CharacterController controller;
    private Vector2 moveInput;
    private Vector3 velocity;
    private bool isGrounded;
    private int jumpsRemaining;
    private bool isDashing;
    private int remainingDashes;
    private float dashEndTime;

    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction dashAction;
    private InputAction reload;
    #endregion

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerInput = new PlayerInput();
        originalSpeed = moveSpeed;
    }

    private void OnEnable()
    {
        moveAction = playerInput.Player.Move;
        moveAction.Enable();

        jumpAction = playerInput.Player.Jump;
        jumpAction.Enable();

        dashAction = playerInput.Player.Dash;
        dashAction.Enable();

        reload = playerInput.Player.Reload;
        reload.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        jumpAction.Disable();
        dashAction.Disable();
    }

    private void FixedUpdate()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
            jumpsRemaining = maxJumps;
            remainingDashes = maxDashes;
        }
    }

    private void Update()
    {
        HandleMovement();
        ApplyGravity();
        if (!dashAction.triggered || remainingDashes <= 0) return;
        StartCoroutine(Dash());
        remainingDashes--;
    }
    public void ApplySlow(float slowPercentage)
    {
        moveSpeed = originalSpeed * (1 - slowPercentage);
    }

    public void RemoveSlow()
    {
        moveSpeed = originalSpeed;
    }
    private void HandleMovement()
    {
        moveInput = moveAction.ReadValue<Vector2>();
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;

        if (isDashing)
        {
            if (Time.time >= dashEndTime)
            {
                isDashing = false;
            }
            else
            {
                controller.Move(move.normalized * (dashSpeed * Time.deltaTime));
                return;
            }
        }

        //---MOVE---
        float targetSpeed = move.magnitude * moveSpeed * currentSpeedModifier;
        float currentSpeed = new Vector3(controller.velocity.x, 0, controller.velocity.z).magnitude;

        currentSpeed = targetSpeed > currentSpeed ? Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.deltaTime) : Mathf.MoveTowards(currentSpeed, targetSpeed, deceleration * Time.deltaTime);

        currentSpeed = Mathf.Clamp(currentSpeed, 0, maxSpeed);

        if (move.magnitude > 0.1f)
        {
            controller.Move(move.normalized * (currentSpeed * Time.deltaTime));
        }

        if (isGrounded)
        {
            controller.Move(move * (moveSpeed * Time.deltaTime));
        }
        else
        {
            //---AIR CONTROL---
            controller.Move(move * (moveSpeed * airControl * Time.deltaTime));
        }

        //---JUMP---
        if (jumpAction.triggered && jumpsRemaining > 0)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
            jumpsRemaining--;
        }
    }

    private void ApplyGravity()
    {
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private IEnumerator Dash()
    {
        isDashing = true;
        dashEndTime = Time.time + dashDuration;

        // Đọc input hiện tại để xác định hướng dash
        Vector2 input = moveAction.ReadValue<Vector2>();

        // Dash theo hướng input
        Vector3 dashDirection = transform.TransformDirection(new Vector3(input.x, 0, input.y)).normalized;

        // Tính toán vận tốc dash
        velocity = dashDirection * dashSpeed;

        // Thực hiện dash
        while (Time.time < dashEndTime)
        {
            controller.Move(velocity * Time.deltaTime);
            yield return null;
        }

        // Kết thúc dash
        isDashing = false;
        velocity = Vector3.zero;
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
        ArGunReload.Play("ArGunReload");
        KBGunReload.Play("KBGunReload");
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