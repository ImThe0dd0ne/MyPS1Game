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

        // Make pivot follow target immediately
        cameraPivot.position = target.position;

        // Apply rotations
        cameraPivot.rotation = Quaternion.Euler(mouseY, mouseX, 0);

        // Position camera relative to pivot with offset
        transform.position = cameraPivot.position + cameraPivot.rotation * offset;

        // Look at target
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }

    public Vector3 GetCameraForward()
    {
        return transform.forward;
    }
}
