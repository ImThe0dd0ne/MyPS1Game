using UnityEngine;
[RequireComponent(typeof(CharacterController))]
public class ThirdPersonPlayer : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float sprintSpeed = 10f;
    public float gravity = -9.81f;
    public KeyCode sprintKey = KeyCode.LeftShift;
    [Header("References")]
    public Transform camPivot;
    public Animator animator;
    private CharacterController controller;
    private Vector3 velocity;
    private float currentSpeed;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        currentSpeed = moveSpeed;
    }

    void Update()
    {
        HandleMovement();
        HandleAnimation();
    }

    private void HandleMovement()
    {
        // Get input
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // Camera-relative movement
        Vector3 camForward = camPivot.forward;
        camForward.y = 0f;
        camForward.Normalize();
        Vector3 camRight = camPivot.right;
        camRight.y = 0f;
        camRight.Normalize();
        Vector3 move = camForward * v + camRight * h;

        // Sprint condition: MUST be pressing shift AND moving
        currentSpeed = moveSpeed;
        if (Input.GetKey(sprintKey) && move.magnitude > 0.1f)
        {
            currentSpeed = sprintSpeed;
        }

        // Apply movement if moving
        if (move.magnitude > 0.1f)
        {
            transform.rotation = Quaternion.LookRotation(move);
            controller.Move(move.normalized * currentSpeed * Time.deltaTime);
        }
        else
        {
            controller.Move(Vector3.zero);
        }

        // Gravity
        if (!controller.isGrounded)
            velocity.y += gravity * Time.deltaTime;
        else
            velocity.y = -1f;

        controller.Move(velocity * Time.deltaTime);
    }

    private void HandleAnimation()
    {
        if (animator != null)
        {
            // Get raw input for immediate response
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            float inputMagnitude = new Vector3(horizontal, 0, vertical).magnitude;

            // Sprint condition: MUST be pressing shift AND moving
            bool isSprinting = Input.GetKey(sprintKey) && inputMagnitude > 0.1f;

            // Set animation parameters
            float animationSpeed = isSprinting ? 2.0f : 1.0f;
            animator.SetFloat("Speed", inputMagnitude * animationSpeed);
            animator.SetBool("Sprint", isSprinting);

            // Debug line removed - was spamming console
        }
    }
}