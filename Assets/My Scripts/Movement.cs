using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonPlayer : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 6.5f;
    public float sprintSpeed = 13f;
    public float gravity = -30f;
    public KeyCode sprintKey = KeyCode.LeftShift;

    [Header("Jump Settings")]
    public float jumpHeight = 2f;
    public KeyCode jumpKey = KeyCode.Space;
    public float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;
    private bool isJumping;
    private float jumpTime;

    [Header("Slide Settings")]
    public float slideSpeed = 22f;
    public float slideDeceleration = 3f;
    public float slideTurnSpeed = 5f;
    public KeyCode slideKey = KeyCode.LeftControl;
    private bool isSliding;
    private Vector3 slideDirection;
    private float currentSlideSpeed;
    private float originalHeight;
    private float originalCenterY;

    [Header("References")]
    public Transform camPivot;
    public Animator animator;

    [Header("Ground Detection")]
    public float groundCheckDistance = 0.3f;
    public LayerMask groundLayer;
    private bool isGroundedReliable;

    [Header("State")]
    public bool isDead = false;

    private CharacterController controller;
    private Vector3 velocity;
    private float currentSpeed;
    private Vector3 lastMoveDirection;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        currentSpeed = moveSpeed;
        originalHeight = controller.height;
        originalCenterY = controller.center.y;

        if (animator == null)
            Debug.LogError("No Animator assigned!");
    }

    void Update()
    {
        if (isDead) return;

        GroundCheck();
        CaptureInput();
        HandleMovement();
        HandleJumpAndSlide();
        HandleAnimation();
    }

    private void GroundCheck()
    {
        Vector3 rayStart = transform.position + Vector3.up * 0.1f;
        bool rayCheck = Physics.Raycast(rayStart, Vector3.down, groundCheckDistance, groundLayer);
        bool controllerCheck = controller.isGrounded;
        isGroundedReliable = rayCheck || controllerCheck;

        if (isGroundedReliable && velocity.y < 0 && !isJumping)
            velocity.y = -2f;

        Debug.DrawRay(rayStart, Vector3.down * groundCheckDistance, isGroundedReliable ? Color.green : Color.red);
    }

    private void CaptureInput()
    {
        if (Input.GetKeyDown(jumpKey))
            jumpBufferCounter = jumpBufferTime;
        else
            jumpBufferCounter -= Time.deltaTime;
    }

    private Vector3 ProjectMoveOnGround(Vector3 move)
    {
        if (Physics.Raycast(transform.position + Vector3.up * 0.2f, Vector3.down, out RaycastHit hit, 1.0f, groundLayer))
            move = Vector3.ProjectOnPlane(move, hit.normal);
        return move.normalized;
    }

    private void HandleMovement()
    {
        if (isSliding) return;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 camForward = camPivot.forward;
        Vector3 camRight = camPivot.right;

        // Smooth out camera vertical influence
        camForward.y = Mathf.Lerp(camForward.y, 0f, 0.9f);
        camRight.y = Mathf.Lerp(camRight.y, 0f, 0.9f);

        Vector3 move = camForward * v + camRight * h;
        move = ProjectMoveOnGround(move);

        currentSpeed = (Input.GetKey(sprintKey) && move.magnitude > 0.1f) ? sprintSpeed : moveSpeed;

        if (move.magnitude > 0.1f)
        {
            lastMoveDirection = move;
            Quaternion targetRot = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 12f);
            controller.Move(move * currentSpeed * Time.deltaTime);
        }
    }

    private void HandleJumpAndSlide()
    {
        if (isJumping)
            jumpTime += Time.deltaTime;

        // Jump
        if (jumpBufferCounter > 0f && isGroundedReliable && !isSliding && !isJumping)
        {
            PerformJump();
            jumpBufferCounter = 0f;
        }

        // Slide - hold to continue
        if (Input.GetKey(slideKey) && isGroundedReliable && !isJumping)
        {
            if (!isSliding)
                StartSlide();
            ContinueSlide();
        }
        else if (isSliding)
        {
            EndSlide();
        }

        // Gravity
        if (!isGroundedReliable || isJumping)
            velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

        // Landing
        if (isJumping && isGroundedReliable && velocity.y <= -2f)
        {
            isJumping = false;
            jumpTime = 0f;
            if (animator) animator.SetBool("IsGrounded", true);
        }
    }

    private void PerformJump()
    {
        isJumping = true;
        jumpTime = 0f;
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        if (animator)
        {
            animator.SetBool("IsGrounded", false);
            animator.SetBool("IsSliding", false);
            animator.SetFloat("Speed", 0);
            animator.ResetTrigger("Jump");
            animator.SetTrigger("Jump");
        }
    }

    private void StartSlide()
    {
        isSliding = true;
        currentSlideSpeed = slideSpeed;

        // Calculate direction
        Vector3 inputDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        if (inputDir.magnitude > 0.1f)
        {
            Vector3 camForward = camPivot.forward;
            Vector3 camRight = camPivot.right;
            camForward.y = 0;
            camRight.y = 0;
            camForward.Normalize();
            camRight.Normalize();
            slideDirection = (camForward * inputDir.z + camRight * inputDir.x).normalized;
        }
        else
        {
            slideDirection = lastMoveDirection.normalized;
        }

        // Set rotation immediately
        transform.rotation = Quaternion.LookRotation(slideDirection);

        // Shrink collider
        controller.height = originalHeight * 0.5f;
        controller.center = new Vector3(0, originalCenterY * 0.5f, 0);

        // Animation setup - play at slower speed for smooth look
        if (animator)
        {
            animator.SetBool("IsSliding", true);
            animator.SetFloat("Speed", 0);
            animator.SetBool("Sprint", false);
            animator.applyRootMotion = false;
            animator.speed = 0.7f; // Slow animation for dramatic effect
        }
    }

    private void ContinueSlide()
    {
        // Smooth steering with input
        Vector3 inputDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        if (inputDir.magnitude > 0.1f)
        {
            Vector3 camForward = camPivot.forward;
            Vector3 camRight = camPivot.right;
            camForward.y = 0;
            camRight.y = 0;
            camForward.Normalize();
            camRight.Normalize();

            Vector3 targetDir = (camForward * inputDir.z + camRight * inputDir.x).normalized;

            // Smooth drift
            slideDirection = Vector3.Slerp(slideDirection, targetDir, Time.deltaTime * slideTurnSpeed).normalized;

            // Prevent micro-jitter
            if (Vector3.Angle(slideDirection, targetDir) < 1f)
                slideDirection = targetDir;
        }

        // Smooth rotation
        Quaternion targetRot = Quaternion.LookRotation(slideDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 10f);

        // Deceleration with minimum speed
        currentSlideSpeed = Mathf.Max(
            Mathf.MoveTowards(currentSlideSpeed, 0f, slideDeceleration * Time.deltaTime),
            sprintSpeed * 0.8f // Never slower than this
        );

        // Slope dynamics
        if (Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, out RaycastHit hit, 1f, groundLayer))
        {
            float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);

            if (slopeAngle > 5f)
            {
                // Boost downhill
                float slopeDot = Vector3.Dot(slideDirection, Vector3.down);
                if (slopeDot > 0)
                {
                    currentSlideSpeed += slopeAngle * Time.deltaTime * 3f;
                    currentSlideSpeed = Mathf.Min(currentSlideSpeed, slideSpeed * 1.8f); // Cap max speed
                }

                // Align to slope
                slideDirection = Vector3.ProjectOnPlane(slideDirection, hit.normal).normalized;
            }
        }

        // Apply movement
        Vector3 move = slideDirection * currentSlideSpeed * Time.deltaTime;
        move.y = -8f * Time.deltaTime; // Strong ground stick
        controller.Move(move);
    }

    private void EndSlide()
    {
        isSliding = false;

        // Restore collider
        controller.height = originalHeight;
        controller.center = new Vector3(0, originalCenterY, 0);

        // Reset animation
        if (animator)
        {
            animator.speed = 1f;
            animator.SetBool("IsSliding", false);
        }
    }

    private void HandleAnimation()
    {
        if (animator == null) return;

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        float inputMag = new Vector3(horizontal, 0, vertical).magnitude;
        bool sprinting = Input.GetKey(sprintKey) && inputMag > 0.1f;

        if (!isSliding && !isJumping)
        {
            animator.SetFloat("Speed", inputMag * (sprinting ? 2f : 1f));
            animator.SetBool("Sprint", sprinting);
        }

        if (!isJumping || jumpTime > 0.1f)
        {
            animator.SetBool("IsGrounded", isGroundedReliable);
        }
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;
        isSliding = false;
        isJumping = false;
        velocity = Vector3.zero;
        if (controller) controller.enabled = false;

        if (animator)
        {
            animator.SetFloat("Speed", 0);
            animator.SetBool("Sprint", false);
            animator.SetBool("IsSliding", false);
            animator.SetBool("IsGrounded", true);
            animator.SetTrigger("Death");
        }
    }

    public void Respawn(Vector3 spawnPosition)
    {
        isDead = false;
        transform.position = spawnPosition;
        if (controller) controller.enabled = true;

        velocity = Vector3.zero;
        isSliding = false;
        isJumping = false;
        jumpTime = 0f;
        jumpBufferCounter = 0f;

        if (animator)
        {
            animator.SetFloat("Speed", 0);
            animator.SetBool("Sprint", false);
            animator.SetBool("IsSliding", false);
            animator.SetBool("IsGrounded", true);
            animator.speed = 1f;
        }
    }
}

/*
FINAL POLISHED SETUP:
=====================

WHAT'S DIFFERENT:
✅ Simplified animation control - no complex pause/resume logic
✅ Slide animation plays at 0.7x speed for dramatic, smooth look
✅ Never slows below sprint speed - maintains momentum
✅ Strong downhill boost with speed cap
✅ Ultra-smooth steering with jitter prevention
✅ Instant rotation at slide start for responsiveness
✅ Perfect ground hugging on all terrain

TUNING GUIDE:
=============
slideSpeed = 22f          → Initial burst (18-25 recommended)
slideDeceleration = 3f    → How fast it slows (2-4 recommended)
slideTurnSpeed = 5f       → Steering responsiveness (4-7 recommended)
animator.speed = 0.7f     → Animation playback speed (0.6-0.8 for slow-mo effect)

Minimum slide speed = sprintSpeed * 0.8f (always feels fast)
Maximum slide speed = slideSpeed * 1.8f (downhill cap)

ANIMATOR REQUIREMENTS:
======================
Parameters:
- Speed (Float)
- Sprint (Bool)
- IsGrounded (Bool)
- IsSliding (Bool)
- Jump (Trigger)
- Death (Trigger)

Transitions:
Walk/Run/Sprint → Slide:
  - Condition: IsSliding = true
  - Has Exit Time: NO
  - Transition Duration: 0
  
Slide → Idle/Walk:
  - Condition: IsSliding = false
  - Has Exit Time: NO
  - Transition Duration: 0.2-0.3 (smooth blend)

Jump → Idle:
  - Condition: IsGrounded = true
  - Has Exit Time: YES
  - Exit Time: 0.9
  - Transition Duration: 0.15

CRITICAL SETTINGS:
==================
✓ Animator: Apply Root Motion = OFF
✓ Slide clip: Loop Time = OFF (or ON if you want looping)
✓ Character Controller Slope Limit = 55-60°
✓ Ground Layer properly set in Inspector

FEEL:
=====
This version prioritizes FLOW and SPEED over realism:
- Slide always feels fast (never sluggish)
- Downhill gives satisfying speed boost
- Smooth drift/steering without jitter
- Animation plays at cinematic 0.7x speed
- No complex pause/resume - just pure momentum

Perfect for action games like Apex, Titanfall, Warframe style movement!
*/