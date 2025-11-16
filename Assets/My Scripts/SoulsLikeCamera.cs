using UnityEngine;

public class SoulsLikeCamera : MonoBehaviour
{
    public Transform target;          // Drag Knight here
    public Transform cameraPivot;     // Drag CameraPivot here
    public float rotationSpeed = 3f;
    public float verticalSpeed = 2f;
    public Vector3 offset = new Vector3(0, 2, -5);

    // Soulslike additions
    public float minVerticalAngle = -30f;
    public float maxVerticalAngle = 60f;
    public float smoothTime = 0.1f;

    [Header("Ground Safety")]
    public LayerMask groundLayer;     // assign WhatIsGround
    public float cameraGroundOffset = 0.2f; // how far above ground the camera will stay
    public float cameraCollisionRadius = 0.35f; // for spherecast safety

    private float mouseX;
    private float mouseY;
    private Vector3 currentVelocity;

    void Start()
    {
        if (target == null)
        {
            UnityEngine.Debug.LogError("Target not assigned to CameraController!");
            return;
        }
        if (cameraPivot == null)
        {
            UnityEngine.Debug.LogError("CameraPivot not assigned to CameraController!");
            return;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        cameraPivot.position = target.position;
        transform.position = cameraPivot.position + offset;
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }

    void LateUpdate()
    {
        if (target == null || cameraPivot == null) return;

        // Toggle cursor lock/unlock
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        // Input rotation
        mouseX += Input.GetAxis("Mouse X") * rotationSpeed;
        mouseY -= Input.GetAxis("Mouse Y") * verticalSpeed;
        mouseY = Mathf.Clamp(mouseY, minVerticalAngle, maxVerticalAngle);

        // Smooth follow
        Vector3 targetPosition = target.position;
        cameraPivot.position = Vector3.SmoothDamp(cameraPivot.position, targetPosition, ref currentVelocity, smoothTime);

        // Apply rotations
        cameraPivot.rotation = Quaternion.Euler(mouseY, mouseX, 0);

        // Desired camera position
        Vector3 desiredPosition = cameraPivot.position + cameraPivot.rotation * offset;

        // Prevent camera from clipping under terrain: use SphereCast toward desired position
        Vector3 dir = (desiredPosition - cameraPivot.position);
        float maxDist = dir.magnitude;
        if (maxDist > 0.001f)
        {
            dir.Normalize();
            if (Physics.SphereCast(cameraPivot.position, cameraCollisionRadius, dir, out RaycastHit hit, maxDist, groundLayer))
            {
                // move camera to just above the hit point
                Vector3 hitPos = hit.point + Vector3.up * cameraGroundOffset;
                // ensure camera doesn't pop inside the pivot
                float safeDist = Mathf.Max(0.5f, (hitPos - cameraPivot.position).magnitude - 0.1f);
                desiredPosition = cameraPivot.position + dir * safeDist;
                desiredPosition.y = Mathf.Max(desiredPosition.y, hit.point.y + cameraGroundOffset);
            }
            else
            {
                // still ensure we are above terrain directly under desired position
                if (Physics.Raycast(desiredPosition + Vector3.up * 0.5f, Vector3.down, out RaycastHit downHit, 10f, groundLayer))
                {
                    float minY = downHit.point.y + cameraGroundOffset;
                    desiredPosition.y = Mathf.Max(desiredPosition.y, minY);
                }
            }
        }

        // Place camera (no extra zoom-out)
        transform.position = desiredPosition;

        // Look slightly above player feet for better framing
        Vector3 lookTarget = target.position + Vector3.up * 1.5f;
        transform.LookAt(lookTarget);
    }

    public Vector3 GetCameraForward()
    {
        return transform.forward;
    }
}
