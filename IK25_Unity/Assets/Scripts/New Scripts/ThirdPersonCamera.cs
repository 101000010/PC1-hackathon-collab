using UnityEngine;
using PlayBionic.MyoOsu.Management; // Add this to reference the DualObjectMover class

public class ThirdPersonCamera : MonoBehaviour
{
    public float turnSpeed = 4.0f; // Speed of camera rotation
    public GameObject target; // The object the camera follows
    private float targetDistance; // Distance between the camera and the target
    public float minTurnAngle = -90.0f; // Minimum vertical angle
    public float maxTurnAngle = 0.0f; // Maximum vertical angle
    private float rotX; // Vertical rotation (pitch)
    private float rotY; // Horizontal rotation (yaw)

    // Reference to the DualMyoObjectMover script
    public DualMyoObjectMover dualObjectMover;

    void Start()
    {
        if (target == null)
        {
            Debug.LogError("Target is not assigned in ThirdPersonCamera.");
            return;
        }

        targetDistance = Vector3.Distance(transform.position, target.transform.position);

        if (dualObjectMover == null)
        {
            Debug.LogError("DualObjectMover is not assigned in ThirdPersonCamera.");
        }

        // Initialize rotation values
        rotX = transform.eulerAngles.x;
        rotY = transform.eulerAngles.y;
    }

    void Update()
    {
        if (dualObjectMover == null || target == null) return;

        // Get the inputs from the DualObjectMover script
        float horizontalInput = dualObjectMover.GetHorizontalInput();
        float verticalInput = dualObjectMover.GetVerticalInput();

        // Update horizontal and vertical rotation
        rotY += horizontalInput * turnSpeed; // Horizontal rotation (yaw)
        rotX += verticalInput * turnSpeed; // Vertical rotation (pitch)

        // Clamp the vertical rotation to prevent flipping
        rotX = Mathf.Clamp(rotX, minTurnAngle, maxTurnAngle);

        // Apply the rotation to the camera
        transform.rotation = Quaternion.Euler(-rotX, rotY, 0);

        // Maintain the camera's position relative to the target
        Vector3 targetPosition = target.transform.position - (transform.forward * targetDistance);
        transform.position = targetPosition;
    }
}
