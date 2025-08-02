using UnityEngine;

public class PCCharacterController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;
    public float groundCheckDistance = 0.2f;

    [Header("Animation Settings")]
    public float animationSmoothTime = 0.1f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public LayerMask groundMask;

    // Components
    private CharacterController controller;
    private Animator animator;

    // Movement variables
    private Vector3 velocity;
    private bool isGrounded;
    private bool isRunning;

    // Animation parameter IDs (for performance)
    private int speedHash;
    private int horizontalHash;
    private int verticalHash;
    private int isGroundedHash;
    private int isJumpingHash;
    private int isRunningHash;

    void Start()
    {
        // Get components
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        // Cache animator parameter hashes
        speedHash = Animator.StringToHash("Speed");
        horizontalHash = Animator.StringToHash("Horizontal");
        verticalHash = Animator.StringToHash("Vertical");
        isGroundedHash = Animator.StringToHash("IsGrounded");
        isJumpingHash = Animator.StringToHash("IsJumping");
        isRunningHash = Animator.StringToHash("IsRunning");

        // Create ground check if it doesn't exist
        if (groundCheck == null)
        {
            GameObject groundCheckObj = new GameObject("GroundCheck");
            groundCheckObj.transform.SetParent(transform);
            groundCheckObj.transform.localPosition = Vector3.down * (controller.height / 2);
            groundCheck = groundCheckObj.transform;
        }
    }

    void Update()
    {
        GroundCheck();
        HandleMovement();
        HandleJump();
        UpdateAnimations();
    }

    void GroundCheck()
    {
        // Check if player is grounded
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckDistance, groundMask);

        // Reset velocity if grounded and falling
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Small negative value to keep grounded
        }
    }

    void HandleMovement()
    {
        // Get input
        float horizontal = Input.GetAxis("Horizontal"); // A/D or Left/Right arrows
        float vertical = Input.GetAxis("Vertical");     // W/S or Up/Down arrows

        // Check if running (hold Shift)
        isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        // Calculate movement direction
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            // Calculate target angle based on camera direction
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;

            // Smooth rotation towards movement direction
            float angle = Mathf.LerpAngle(transform.eulerAngles.y, targetAngle, Time.deltaTime * 10f);
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);

            // Move in the direction we're facing
            Vector3 moveDir = Quaternion.AngleAxis(targetAngle, Vector3.up) * Vector3.forward;

            // Apply speed
            float currentSpeed = isRunning ? runSpeed : walkSpeed;
            controller.Move(moveDir.normalized * currentSpeed * Time.deltaTime);
        }
    }

    void HandleJump()
    {
        // Jump input (Spacebar)
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void UpdateAnimations()
    {
        if (animator == null) return;

        // Get input for animations
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Calculate speed for animation
        Vector3 horizontalVelocity = new Vector3(controller.velocity.x, 0, controller.velocity.z);
        float speed = horizontalVelocity.magnitude;

        // Normalize speed (0 = idle, 0.5 = walk, 1 = run)
        float normalizedSpeed = 0f;
        if (speed > 0.1f)
        {
            normalizedSpeed = isRunning ? 1f : 0.5f;
        }

        // Update animator parameters
        animator.SetFloat(speedHash, normalizedSpeed, animationSmoothTime, Time.deltaTime);
        animator.SetFloat(horizontalHash, horizontal, animationSmoothTime, Time.deltaTime);
        animator.SetFloat(verticalHash, vertical, animationSmoothTime, Time.deltaTime);
        animator.SetBool(isGroundedHash, isGrounded);
        animator.SetBool(isJumpingHash, velocity.y > 0.1f);
        animator.SetBool(isRunningHash, isRunning && speed > 0.1f);
    }

    // Draw ground check in scene view
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckDistance);
        }
    }
}