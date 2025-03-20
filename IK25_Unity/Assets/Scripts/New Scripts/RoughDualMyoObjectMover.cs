using UnityEngine;

namespace PlayBionic.MyoOsu.Management
{
    public class RoughDualMyoObjectMover : MonoBehaviour
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
            float leftValue = rmsValues[leftChannel];
            float rightValue = rmsValues[rightChannel];

            horizontalInput = (rightValue - leftValue) * maxSpeed;

            // Use the average of left and right channels for forward/backward movement (z-axis)
            forwardInput = ((leftValue + rightValue) / 2) * maxSpeed;
        }

        private void HandleRightArmData(float[] rmsValues, double timestamp)
        {
            if (calibration == null || !calibration.IsCalibrated) return;

            // Get the primary channels for right arm (up/down movement)
            int upChannel = calibration.GetRightArmUpChannel();
            int downChannel = calibration.GetRightArmDownChannel();

            // Calculate vertical input based on the difference between up and down channels
            float upValue = rmsValues[upChannel];
            float downValue = rmsValues[downChannel];

            verticalInput = (upValue - downValue) * maxSpeed;

            // Use the average of up and down channels for forward/backward movement (z-axis)
            forwardInput = ((upValue + downValue) / 2) * maxSpeed;
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