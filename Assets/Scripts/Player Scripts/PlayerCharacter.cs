using UnityEngine;

//[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 10f;
    public float rotationSpeed = 720f;

    [Header("Jump Settings")]
    public float jumpHeight = 2f;
    public float gravity = -20f;

    [Header("Dash Settings")]
    public float dashDistance = 10f;
    public float dashCooldown = 1f;
    public float dashDuration = 0.2f;

    public Transform cameraTransform;
    private CharacterController controller;
    private Camera mainCamera;

    private Vector3 velocity;
    private Vector3 dashDirection;

    private bool isGrounded;
    private bool isDashing = false;
    private float dashTime;
    private float lastDashTime;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        mainCamera = Camera.main;
    }
    void Update()
    {
        HandleMovement();
        HandleJump();
        ApplyGravity();
        HandleLook();
    }
    void HandleMovement()
    {
        isGrounded = controller.isGrounded;

        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        input = Vector2.ClampMagnitude(input, 1f);

        // Convert input to camera-relative direction
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0f; right.y = 0f;
        forward.Normalize(); right.Normalize();

        Vector3 moveDir = forward * input.y + right * input.x;
        HandleDash(moveDir);

        if (moveDir != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, 720f * Time.deltaTime);
        }

        Vector3 move = moveDir * moveSpeed;
        if (isDashing)
        {
            move = transform.forward * (dashDistance / dashDuration);
        }

        controller.Move((move + velocity) * Time.deltaTime);
    }

    void HandleJump()
    {
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isDashing)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }
    void ApplyGravity()
    {
        if (!isDashing)
            velocity.y += gravity * Time.deltaTime;
    }
    void HandleDash(Vector3 moveDirection)
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time >= lastDashTime + dashCooldown && !isDashing)
        {
            isDashing = true;
            dashTime = Time.time;
            lastDashTime = Time.time;

            dashDirection = moveDirection.normalized;
            dashDirection.y = 0f;
        }

        if (isDashing && Time.time >= dashTime + dashDuration)
            isDashing = false;
    }
    void HandleLook()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        if (groundPlane.Raycast(ray, out float distance))
        {
            Vector3 hitPoint = ray.GetPoint(distance);
            Vector3 lookDir = hitPoint - transform.position;
            lookDir.y = 0f;

            if (lookDir != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDir);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 720f * Time.deltaTime);
            }
        }
    }
}