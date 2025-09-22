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

    private float mouseX;
    private float mouseY;
    private Vector3 currentVelocity;

    void Start()
    {
        if (target == null)
        {
            Debug.LogError("Target not assigned to CameraController!");
            return;
        }
        if (cameraPivot == null)
        {
            Debug.LogError("CameraPivot not assigned to CameraController!");
            return;
        }

        // Lock cursor for soulslike feel
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Initial setup
        cameraPivot.position = target.position;
        transform.position = cameraPivot.position + offset;
        transform.LookAt(target.position);
    }

    void LateUpdate()
    {
        if (target == null || cameraPivot == null) return;

        // Handle cursor lock/unlock
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

        // Get mouse input for rotation (both horizontal and vertical)
        mouseX += Input.GetAxis("Mouse X") * rotationSpeed;
        mouseY -= Input.GetAxis("Mouse Y") * verticalSpeed;

        // Clamp vertical rotation
        mouseY = Mathf.Clamp(mouseY, minVerticalAngle, maxVerticalAngle);

        // Make pivot follow target smoothly
        Vector3 targetPosition = target.position;
        cameraPivot.position = Vector3.SmoothDamp(cameraPivot.position, targetPosition, ref currentVelocity, smoothTime);

        // Apply rotations to pivot
        cameraPivot.rotation = Quaternion.Euler(mouseY, mouseX, 0);

        // Position camera relative to pivot with offset
        Vector3 desiredPosition = cameraPivot.position + cameraPivot.rotation * offset;
        transform.position = desiredPosition;

        // Make camera look at target (slightly above feet for better framing)
        Vector3 lookTarget = target.position + Vector3.up * 1.5f;
        transform.LookAt(lookTarget);
    }

    // Public method for interaction system to get camera forward direction
    public Vector3 GetCameraForward()
    {
        return transform.forward;
    }
}