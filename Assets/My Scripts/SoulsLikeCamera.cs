using UnityEngine;

public class SoulsLikeCamera : MonoBehaviour
{
    public Transform target;
    public Transform cameraPivot;
    public float rotationSpeed = 3f;
    public float verticalSpeed = 2f;
    public Vector3 offset = new Vector3(0, 2, -5);

    [Header("Angle Limits")]
    public float minVerticalAngle = -30f;
    public float maxVerticalAngle = 60f;
    public float smoothTime = 0.1f;

    [Header("Collision & Occlusion")]
    public LayerMask collisionLayers;
    public float minDistance = 1f;
    public float terrainHeightOffset = 1.5f;
    public float collisionRadius = 0.3f;

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

        mouseX += Input.GetAxis("Mouse X") * rotationSpeed;
        mouseY -= Input.GetAxis("Mouse Y") * verticalSpeed;
        mouseY = Mathf.Clamp(mouseY, minVerticalAngle, maxVerticalAngle);

        cameraPivot.position = target.position;
        cameraPivot.rotation = Quaternion.Euler(mouseY, mouseX, 0);

        Vector3 desiredPosition = cameraPivot.position + cameraPivot.rotation * offset;
        Vector3 finalPosition = HandleCollisions(desiredPosition);

        transform.position = finalPosition;
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }

    Vector3 HandleCollisions(Vector3 desiredPosition)
    {
        Vector3 direction = desiredPosition - cameraPivot.position;
        float distance = direction.magnitude;

        RaycastHit hit;
        if (Physics.SphereCast(cameraPivot.position, collisionRadius, direction.normalized, out hit, distance, collisionLayers))
        {
            desiredPosition = hit.point - direction.normalized * collisionRadius;
            distance = (desiredPosition - cameraPivot.position).magnitude;
            
            if (distance < minDistance)
            {
                desiredPosition = cameraPivot.position + direction.normalized * minDistance;
            }
        }

        if (Physics.Raycast(desiredPosition, Vector3.down, out hit, 100f, collisionLayers))
        {
            float minHeightAboveTerrain = hit.point.y + terrainHeightOffset;
            if (desiredPosition.y < minHeightAboveTerrain)
            {
                desiredPosition.y = minHeightAboveTerrain;
            }
        }

        return desiredPosition;
    }

    public Vector3 GetCameraForward()
    {
        return transform.forward;
    }
}
