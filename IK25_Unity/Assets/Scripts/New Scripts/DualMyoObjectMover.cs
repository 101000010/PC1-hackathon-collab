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
        private float smoothingFactor = 0.1f; // Smoothing factor for input values

        private float horizontalInput = 0f; // Left/right movement (controlled by left arm)
        private float verticalInput = 0f;   // Up/down movement (controlled by right arm)
        private float forwardInput = 0f;   // Forward/backward movement (based on muscle strength)

        private Vector3 movementDirection = Vector3.zero;

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

            // Calculate horizontal input based on the difference between left and right channels
            float leftValue = Mathf.Clamp(rmsValues[leftChannel], 0, 1);
            float rightValue = Mathf.Clamp(rmsValues[rightChannel], 0, 1);

            float targetHorizontalInput = (rightValue - leftValue); // Positive for right, negative for left
            horizontalInput = Mathf.Lerp(horizontalInput, targetHorizontalInput * maxSpeed, smoothingFactor);

            // Use the average of left and right channels for forward/backward movement (z-axis)
            float targetForwardInput = (leftValue + rightValue) / 2; // Average strength
            forwardInput = Mathf.Lerp(forwardInput, targetForwardInput * maxSpeed, smoothingFactor);
        }

        private void HandleRightArmData(float[] rmsValues, double timestamp)
        {
            if (calibration == null || !calibration.IsCalibrated) return;

            // Get the primary channels for right arm (up/down movement)
            int upChannel = calibration.GetRightArmUpChannel();
            int downChannel = calibration.GetRightArmDownChannel();

            // Calculate vertical input based on the difference between up and down channels
            float upValue = Mathf.Clamp(rmsValues[upChannel], 0, 1);
            float downValue = Mathf.Clamp(rmsValues[downChannel], 0, 1);

            float targetVerticalInput = (upValue - downValue); // Positive for up, negative for down
            verticalInput = Mathf.Lerp(verticalInput, targetVerticalInput * maxSpeed, smoothingFactor);

            // Use the average of up and down channels for forward/backward movement (z-axis)
            float targetForwardInput = (upValue + downValue) / 2; // Average strength
            forwardInput = Mathf.Lerp(forwardInput, targetForwardInput * maxSpeed, smoothingFactor);
        }

        // Public method to get vertical input
        public float GetVerticalInput()
        {
            return verticalInput;
        }

        // Public method to get horizontal input
        public float GetHorizontalInput()
        {
            return horizontalInput;
        }

        private void Update()
        {
            // Do not move the object if calibration is not complete
            if (calibration == null || !calibration.IsCalibrated) return;

            // Calculate movement direction based on the inputs
            movementDirection = new Vector3(horizontalInput, verticalInput, forwardInput); // Left/right (x), up/down (y), forward/backward (z)

            // Apply movement
            if (objectToMove != null)
            {
                objectToMove.Translate(movementDirection * Time.deltaTime);

                // Debug the movement direction
                Debug.Log($"Movement Direction: {movementDirection}");
            }
        }
    }
}