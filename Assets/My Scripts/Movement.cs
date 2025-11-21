using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonPlayer : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 6.5f;
    public float sprintSpeed = 13f;
    public float gravity = -30f;
    public KeyCode sprintKey = KeyCode.LeftShift;
    
    [Header("AAA Responsiveness")]
    [Tooltip("How fast character turns (higher = snappier)")]
    public float rotationSpeed = 18f;
    [Tooltip("Input smoothing (lower = more responsive, 0 = instant)")]
    public float inputSmoothTime = 0.08f;
    [Tooltip("Animation blend speed (lower = faster transitions)")]
    public float animationDampTime = 0.05f;

    [Header("Jump Settings")]
    public float jumpHeight = 2f;
    public KeyCode jumpKey = KeyCode.Space;
    public float jumpBufferTime = 0.2f;

    [Header("Slide (steerable, hold-to-extend)")]
    public KeyCode slideKey = KeyCode.LeftControl;
    [Tooltip("Initial speed burst when starting slide")]
    public float slideStartBoost = 26f;
    [Tooltip("How quickly slide speed decays when NOT holding")]
    public float slideDecay = 9f;
    [Tooltip("How slowly slide speed decays when holding (you can maintain momentum)")]
    public float slideDecayWhenHolding = 1.8f;
    [Tooltip("How responsive steering is while sliding")]
    public float slideSteerSpeed = 8f;
    [Tooltip("Minimum slide speed before auto-ending (when not held)")]
    public float minSlideEndSpeed = 3f;
    [Tooltip("Collider height scale while sliding (0..1)")]
    public float slideHeightScale = 0.55f;
    [Tooltip("If true, slide will pause the animation at 'slideHoldNormalizedTime' while holding the key")]
    public bool pauseAnimationWhileHeld = true;
    [Range(0f, 1f)]
    public float slideHoldNormalizedTime = 0.12f; // where to pause animation (0=start,1=end)

    [Header("References")]
    public Transform camPivot; // camera pivot (for relative inputs)
    public Animator animator;  // Animator - must have "Slide" state/clip
    public LayerMask groundLayer; // your WhatIsGround layer

    private CharacterController controller;
    private Vector3 velocity;
    private float jumpBufferCounter;
    private bool isGrounded;
    private bool isJumping;
    private bool isSliding;
    private bool isSlideHeld;
    private bool isDead;
    private Vector3 slideDirection;
    private float slideSpeed;
    private float originalHeight;
    private Vector3 originalCenter;
    private Coroutine slideCoroutine;
    private FixedCombatSystem combatSystem;
    
    private Vector2 currentInputVector;
    private Vector2 smoothInputVelocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        originalHeight = controller.height;
        originalCenter = controller.center;
        combatSystem = GetComponent<FixedCombatSystem>();

        if (animator == null)
            UnityEngine.Debug.LogWarning("Animator not assigned on ThirdPersonPlayer.");
    }

    void Update()
    {
        if (isDead) return;
        
        GroundCheck();
        CaptureInput();

        if (isSliding)
        {
            isSlideHeld = Input.GetKey(slideKey);
            return;
        }

        Move();
        HandleJump();
        ApplyGravity();
        UpdateAnimatorMove();
    }

    private void GroundCheck()
    {
        Vector3 origin = transform.position + Vector3.up * 0.1f;
        isGrounded = Physics.Raycast(origin, Vector3.down, 0.3f, groundLayer) || controller.isGrounded;
        if (isGrounded && velocity.y < 0f)
            velocity.y = -2f;
    }

    private void CaptureInput()
    {
        if (Input.GetKeyDown(jumpKey))
            jumpBufferCounter = jumpBufferTime;
        else
            jumpBufferCounter -= Time.deltaTime;

        if (Input.GetKeyDown(slideKey) && isGrounded && !isSliding && !isJumping)
        {
            StartSlide();
        }
    }

    private void Move()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector2 targetInput = new Vector2(h, v);
        currentInputVector = Vector2.SmoothDamp(currentInputVector, targetInput, ref smoothInputVelocity, inputSmoothTime);

        Vector3 camForward = camPivot.forward;
        Vector3 camRight = camPivot.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveInput = camForward * currentInputVector.y + camRight * currentInputVector.x;

        if (moveInput.sqrMagnitude > 1f)
            moveInput.Normalize();

        if (moveInput.sqrMagnitude > 0.01f)
        {
            if (Physics.Raycast(transform.position + Vector3.up * 0.2f, Vector3.down, out RaycastHit hit, 1.5f, groundLayer))
                moveInput = Vector3.ProjectOnPlane(moveInput, hit.normal).normalized;

            bool sprinting = Input.GetKey(sprintKey);
            float speed = sprinting ? sprintSpeed : moveSpeed;

            Quaternion targetRot = Quaternion.LookRotation(moveInput);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotationSpeed);

            controller.Move(moveInput * speed * Time.deltaTime);
        }
    }



    private void HandleJump()
    {
        if (jumpBufferCounter > 0f && isGrounded && !isJumping && !isSliding)
        {
            isJumping = true;
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            jumpBufferCounter = 0f;

            if (animator)
            {
                animator.ResetTrigger("Jump");
                animator.SetTrigger("Jump");
                animator.SetBool("IsGrounded", false);
            }
        }

        if (isGrounded && isJumping && velocity.y <= 0f)
        {
            isJumping = false;
            if (animator) animator.SetBool("IsGrounded", true);
        }
    }

    private void ApplyGravity()
    {
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void UpdateAnimatorMove()
    {
        if (animator == null) return;

        float inputMag = currentInputVector.magnitude;
        bool sprinting = Input.GetKey(sprintKey) && inputMag > 0.1f;

        if (!isSliding && !isJumping)
        {
            float speedValue = inputMag * (sprinting ? 2f : 1f);
            animator.SetFloat("Speed", speedValue, animationDampTime, Time.deltaTime);
            animator.SetBool("Sprint", sprinting);
        }

        if (!isJumping)
            animator.SetBool("IsGrounded", isGrounded);
    }

    // ------------------- SLIDE (steerable + holdable) -------------------

    private void StartSlide()
    {
        isSliding = true;
        isSlideHeld = true;
        slideSpeed = slideStartBoost;

        // Lock initial direction from input or player's facing
        Vector3 inputDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
        if (inputDir.sqrMagnitude > 0.01f)
        {
            Vector3 camForward = camPivot.forward; camForward.y = 0f; camForward.Normalize();
            Vector3 camRight = camPivot.right; camRight.y = 0f; camRight.Normalize();
            slideDirection = (camForward * inputDir.z + camRight * inputDir.x).normalized;
        }
        else
        {
            slideDirection = transform.forward;
        }

        // rotate toward slide direction (visually)
        transform.rotation = Quaternion.LookRotation(slideDirection);

        // shrink collider so player appears low to ground
        controller.height = originalHeight * slideHeightScale;
        controller.center = originalCenter * slideHeightScale;

        if (animator)
        {
            animator.SetBool("IsSliding", true);
            // Crossfade into the Slide animation for smooth blending
            animator.CrossFade("Slide", 0.08f);
            // Ensure animator initially runs so we can then freeze at the right moment
            animator.speed = 1f;
        }

        // Stop previous coroutine if any
        if (slideCoroutine != null) StopCoroutine(slideCoroutine);
        slideCoroutine = StartCoroutine(SlideLoop());
    }

    private IEnumerator SlideLoop()
    {
        // Slide loop keeps running while isSliding true.
        // It allows steering but with limited responsiveness (drift), slope momentum, and hold-to-maintain.
        bool animationFrozen = false;

        while (isSliding)
        {
            // Steering input (you can steer/strafe while sliding; adjust responsiveness via slideSteerSpeed)
            Vector3 inputDir = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
            if (inputDir.sqrMagnitude > 0.01f)
            {
                Vector3 camForward = camPivot.forward; camForward.y = 0f; camForward.Normalize();
                Vector3 camRight = camPivot.right; camRight.y = 0f; camRight.Normalize();
                Vector3 desired = (camForward * inputDir.z + camRight * inputDir.x).normalized;

                // Slerp towards desired direction — this produces a smooth drift/steer
                slideDirection = Vector3.Slerp(slideDirection, desired, Time.deltaTime * slideSteerSpeed).normalized;

                // Optionally rotate body slowly toward direction for visuals
                Quaternion look = Quaternion.LookRotation(slideDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, look, Time.deltaTime * 8f);
            }

            // Slope interaction: cast down to find normal & slope angle
            if (Physics.Raycast(transform.position + Vector3.up * 0.2f, Vector3.down, out RaycastHit hit, 1.2f, groundLayer))
            {
                // Project movement onto slope to stick to terrain
                slideDirection = Vector3.ProjectOnPlane(slideDirection, hit.normal).normalized;

                // Arcade boost: increase slideSpeed if going downhill (dot with downhill)
                Vector3 downhill = -hit.normal; // points down the slope
                float downhillDot = Vector3.Dot(slideDirection, downhill);
                if (downhillDot > 0.2f)
                {
                    // boost more the steeper and the more aligned we are with downhill
                    float boost = downhillDot * (1f + Vector3.Angle(hit.normal, Vector3.up) / 20f) * Time.deltaTime * 6f;
                    slideSpeed += boost;
                    // cap a reasonable max
                    slideSpeed = Mathf.Min(slideSpeed, slideStartBoost * 2f);
                }
            }

            // decay slide speed depending on whether player holds slide
            float decay = isSlideHeld ? slideDecayWhenHolding : slideDecay;
            slideSpeed = Mathf.MoveTowards(slideSpeed, 0f, decay * Time.deltaTime);

            // movement
            controller.Move(slideDirection * slideSpeed * Time.deltaTime);

            // gravity to keep contact with ground
            velocity.y += gravity * Time.deltaTime;
            controller.Move(Vector3.up * velocity.y * Time.deltaTime);

            // Animation freeze/resume logic (only if requested)
            if (animator != null && pauseAnimationWhileHeld)
            {
                AnimatorStateInfo st = animator.GetCurrentAnimatorStateInfo(0);
                if (!animationFrozen && st.IsName("Slide"))
                {
                    // once animation passes normalized time threshold and player is holding slide, freeze there
                    if (st.normalizedTime >= slideHoldNormalizedTime && isSlideHeld)
                    {
                        animator.Play("Slide", 0, slideHoldNormalizedTime); // set to exact frame
                        animator.speed = 0f; // pause
                        animationFrozen = true;
                    }
                }

                // If frozen and player releases slide, resume animator and let it continue to end
                if (animationFrozen && !isSlideHeld)
                {
                    animator.speed = 1f;
                    animationFrozen = false;
                }
            }

            // End conditions:
            // - If player releases and slideSpeed falls under minimum threshold => end
            // - If player is grounded false or other safety checks (not added here but can be)
            if (!isSlideHeld && slideSpeed <= minSlideEndSpeed)
            {
                // Resume animation (if paused) then finish slide
                if (animator != null) animator.speed = 1f;
                break;
            }

            // If player explicitly released slide and slideSpeed is low, break too (already handled)
            // Wait a frame
            yield return null;
        }

        // Clean-up and smooth exit
        EndSlide();
    }

    private void EndSlide()
    {
        if (!isSliding) return;

        isSliding = false;
        isSlideHeld = false;

        // restore collider
        controller.height = originalHeight;
        controller.center = originalCenter;

        // resume animation and clear slide flag
        if (animator)
        {
            animator.speed = 1f;
            animator.SetBool("IsSliding", false);
            // crossfade to idle/walk will be handled by animator transitions using Speed param
        }

        // ensure slide coroutine cleared
        if (slideCoroutine != null)
        {
            StopCoroutine(slideCoroutine);
            slideCoroutine = null;
        }
    }

    // Death & respawn (kept simple)
    public void Die()
    {
        isDead = true;
        isSliding = false;
        isJumping = false;
        velocity = Vector3.zero;
        if (controller) controller.enabled = false;

        if (animator)
        {
            animator.SetTrigger("Die");
            animator.SetFloat("Speed", 0f);
            animator.SetBool("Sprint", false);
            animator.SetBool("IsSliding", false);
        }
    }

    public void Respawn(Vector3 spawnPosition)
    {
        isDead = false;
        transform.position = spawnPosition;
        velocity = Vector3.zero;
        isSliding = false;
        isJumping = false;
        if (controller) controller.enabled = true;

        if (animator)
        {
            animator.Rebind();
            animator.Update(0f);
        }
    }
}
