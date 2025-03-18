using UnityEngine;
using PlayBionic.MyoOsu.Management; // Add this to reference the DualObjectMover class

public class ThirdPersonCamera : MonoBehaviour
{
    public float turnSpeed = 4.0f;
    public GameObject target;
    private float targetDistance;
    public float minTurnAngle = -90.0f;
    public float maxTurnAngle = 0.0f;
    private float rotX;

    // Reference to the DualObjectMover script
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
    }

    void Update()
    {
        if (dualObjectMover == null || target == null) return;

        // Get the inputs from the DualObjectMover script
        float y = dualObjectMover.GetHorizontalInput() * turnSpeed;
        rotX += dualObjectMover.GetVerticalInput() * turnSpeed;

        // Clamp the vertical rotation
        rotX = Mathf.Clamp(rotX, minTurnAngle, maxTurnAngle);

        // Rotate the camera
        transform.eulerAngles = new Vector3(-rotX, transform.eulerAngles.y + y, 0);

        // Move the camera position
        transform.position = target.transform.position - (transform.forward * targetDistance);
    }
}
