using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;          // Drag Knight here
    public Transform cameraPivot;     // Drag CameraPivot here
    public float rotationSpeed = 3f;
    public Vector3 offset = new Vector3(0, 2, -5);

    private float mouseX;

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

        // Initial setup
        cameraPivot.position = target.position;
        transform.position = cameraPivot.position + offset;
        transform.LookAt(target.position);
    }

    void LateUpdate()
    {
        if (target == null || cameraPivot == null) return;

        // Get mouse input for rotation
        mouseX += Input.GetAxis("Mouse X") * rotationSpeed;

        // Make pivot follow target
        cameraPivot.position = target.position;

        // Rotate pivot based on mouse input
        cameraPivot.rotation = Quaternion.Euler(0, mouseX, 0);

        // Position camera relative to pivot with offset
        transform.position = cameraPivot.position + cameraPivot.rotation * offset;

        // Make camera look at target (slightly above feet)
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}