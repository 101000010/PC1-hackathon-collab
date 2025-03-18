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

        private float horizontalInput = 0f; // Left/right movement (controlled by left arm)
        private float verticalInput = 0f;   // Up/down movement (controlled by right arm)

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

            // Get the primary channel for the left arm
            int primaryChannel = calibration.GetLeftArmPrimaryChannel();

            // Use the RMS value of the primary channel for horizontal input (e.g., left/right movement)
            horizontalInput = Mathf.Clamp(rmsValues[primaryChannel], 0, 1) * maxSpeed;
        }

        private void HandleRightArmData(float[] rmsValues, double timestamp)
        {
            if (calibration == null || !calibration.IsCalibrated) return;

            // Get the primary channel for the right arm
            int primaryChannel = calibration.GetRightArmPrimaryChannel();

            // Use the RMS value of the primary channel for vertical input (e.g., up/down movement)
            verticalInput = Mathf.Clamp(rmsValues[primaryChannel], 0, 1) * maxSpeed;
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
            movementDirection = new Vector3(horizontalInput, verticalInput, 0); // Left/right (x), up/down (y)

            // Apply movement
            if (objectToMove != null)
            {
                objectToMove.Translate(movementDirection * Time.deltaTime);
            }
        }
    }
}