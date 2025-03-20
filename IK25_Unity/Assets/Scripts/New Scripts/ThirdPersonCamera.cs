using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target; // The flying game object to follow

    [Header("Camera Settings")]
    public Vector3 offset = new Vector3(0, 2, -5); // Offset from the target
    public float followSpeed = 5.0f; // Speed for smoothing camera position
    public float rotationSpeed = 5.0f; // Speed for smoothing camera rotation

    private void LateUpdate()
    {
        if (target == null) return;

        // Calculate the desired position based on the target's position and offset
        Vector3 desiredPosition = target.position + target.rotation * offset;

        // Smoothly interpolate the camera's position
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * followSpeed);

        // Calculate the desired rotation to look at the target
        Quaternion desiredRotation = Quaternion.LookRotation(target.position - transform.position);

        // Smoothly interpolate the camera's rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, Time.deltaTime * rotationSpeed);
    }
}