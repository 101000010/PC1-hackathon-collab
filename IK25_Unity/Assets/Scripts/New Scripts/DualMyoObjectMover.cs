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

        private float horizontalInput = 0f;
        private float verticalInput = 0f;

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
            // Calculate the average RMS value for the left arm
            float averageRMS = 0f;
            foreach (float rms in rmsValues)
            {
                averageRMS += rms;
            }
            averageRMS /= rmsValues.Length;

            // Map the average RMS value to vertical input (e.g., forward/backward movement)
            verticalInput = Mathf.Clamp(averageRMS, 0, 1) * maxSpeed;
        }

        private void HandleRightArmData(float[] rmsValues, double timestamp)
        {
            // Calculate the average RMS value for the right arm
            float averageRMS = 0f;
            foreach (float rms in rmsValues)
            {
                averageRMS += rms;
            }
            averageRMS /= rmsValues.Length;

            // Map the average RMS value to horizontal input (e.g., left/right movement)
            horizontalInput = Mathf.Clamp(averageRMS, 0, 1) * maxSpeed;
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

            // Example: Use normalized data for movement
            float[] leftArmData = calibration.NormalizeLeftArmData(new float[8]); // Replace with actual RMS data
            float[] rightArmData = calibration.NormalizeRightArmData(new float[8]); // Replace with actual RMS data

            // Calculate movement direction based on normalized data
            float forwardSpeed = leftArmData[0]; // Example: Use channel 0 for forward movement
            float sidewaysSpeed = rightArmData[0]; // Example: Use channel 0 for sideways movement

            movementDirection = new Vector3(sidewaysSpeed, 0, forwardSpeed) * maxSpeed;

            // Apply movement
            if (objectToMove != null)
            {
                objectToMove.Translate(movementDirection * Time.deltaTime);
            }
        }
    }
}