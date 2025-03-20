using UnityEngine;

namespace PlayBionic.MyoOsu.Management
{
    public class DualMyoObjectMover : MonoBehaviour
    {
        [SerializeField]
        private RMSFilter leftArmRMSFilter; // RMSFilter for the left arm Myo Armband

        [SerializeField]
        private RMSFilter rightArmRMSFilter; // RMSFilter for the right arm Myo Armband

        [SerializeField]
        private Calibration calibration; // Reference to the Calibration script

        [SerializeField]
        private Transform objectToMove; // The object to move in space

        [SerializeField]
        private float maxSpeed = 10f; // Maximum speed of the object

        [SerializeField]
        private float rotationSpeed = 100f; // Speed of rotation

        [SerializeField]
        private float smoothingFactor = 0.1f; // Smoothing factor for input values

        private float horizontalInput = 0f; // Left/right rotation (controlled by left arm)
        private float forwardInput = 0f;   // Forward/backward movement (based on both arms)
        private float verticalInput = 0f; // Up/down movement (based on right arm)

        private Vector3 movementDirection = Vector3.zero;
        private Vector3 rotationDirection = Vector3.zero; // Rotation direction (yaw only)

        private void Awake()
        {
            if (leftArmRMSFilter != null)
            {
                leftArmRMSFilter.OnDataReceived += HandleLeftArmData;
            }

            if (rightArmRMSFilter != null)
            {
                rightArmRMSFilter.OnDataReceived += HandleRightArmData;
            }
        }

        private void OnDestroy()
        {
            if (leftArmRMSFilter != null)
            {
                leftArmRMSFilter.OnDataReceived -= HandleLeftArmData;
            }

            if (rightArmRMSFilter != null)
            {
                rightArmRMSFilter.OnDataReceived -= HandleRightArmData;
            }
        }

        private void HandleLeftArmData(float[] rmsValues, double timestamp)
        {
            if (calibration == null || !calibration.IsCalibrated) return;

            // Get the primary channels for left arm (left/right movement)
            int leftChannel = calibration.GetLeftArmLeftChannel();
            int rightChannel = calibration.GetLeftArmRightChannel();

            // Calculate horizontal rotation (yaw) based on the difference between left and right channels
            float leftValue = Mathf.Clamp(rmsValues[leftChannel], 0, 1);
            float rightValue = Mathf.Clamp(rmsValues[rightChannel], 0, 1);

            float targetYawInput = (rightValue - leftValue); // Positive for right, negative for left
            horizontalInput = Mathf.Lerp(horizontalInput, targetYawInput * rotationSpeed, smoothingFactor);

            // Update rotation direction (yaw only)
            rotationDirection = new Vector3(0, horizontalInput, 0); // Yaw (y-axis only)

            // Use the average of left and right channels for forward movement
            float targetForwardInput = (leftValue + rightValue) / 2; // Average strength
            forwardInput = Mathf.Lerp(forwardInput, targetForwardInput * maxSpeed, smoothingFactor);
        }

        private void HandleRightArmData(float[] rmsValues, double timestamp)
        {
            if (calibration == null || !calibration.IsCalibrated) return;

            // Get the primary channels for right arm (up/down movement)
            int upChannel = calibration.GetRightArmUpChannel();
            int downChannel = calibration.GetRightArmDownChannel();

            // Calculate vertical movement (Y-axis) based on the difference between up and down channels
            float upValue = Mathf.Clamp(rmsValues[upChannel], 0, 1);
            float downValue = Mathf.Clamp(rmsValues[downChannel], 0, 1);

            float targetVerticalInput = (upValue - downValue); // Positive for up, negative for down
            verticalInput = Mathf.Lerp(verticalInput, targetVerticalInput * maxSpeed, smoothingFactor);

            // Combine right arm input with left arm input for forward movement
            float targetForwardInput = (upValue + downValue) / 2; // Average strength
            forwardInput = Mathf.Lerp(forwardInput, targetForwardInput * maxSpeed, smoothingFactor);
        }

        private void Update()
        {
            // Do not move the object if calibration is not complete
            if (calibration == null || !calibration.IsCalibrated) return;

            // Ensure forwardInput is positive for forward movement
            forwardInput = Mathf.Abs(forwardInput);

            // Calculate movement direction based on the inputs
            movementDirection = new Vector3(0, verticalInput, forwardInput); // Up/down (Y-axis), forward/backward (Z-axis)

            // Apply movement
            if (objectToMove != null)
            {
                objectToMove.Translate(movementDirection * Time.deltaTime, Space.World);

                // Apply rotation (yaw only)
                objectToMove.Rotate(rotationDirection * Time.deltaTime, Space.Self);

                // Debug the movement and rotation directions
                Debug.Log($"Movement Direction: {movementDirection}, Rotation Direction: {rotationDirection}");
            }
        }

        public float GetHorizontalInput()
        {
            return horizontalInput;
        }

        public float GetVerticalInput()
        {
            return forwardInput; // Assuming vertical input is represented by forwardInput
        }
    }
}