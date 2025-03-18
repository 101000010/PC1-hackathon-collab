using UnityEngine;

namespace PlayBionic.MyoOsu.Management
{
    public class ObjectMover : MonoBehaviour
    {
        [SerializeField]
        private RMSFilter rmsFilter; // Reference to the RMSFilter script

        [SerializeField]
        private Transform objectToMove; // The object to move forward

        [SerializeField]
        private float maxSpeed = 10f; // Maximum speed of the object

        private void Awake()
        {
            if (rmsFilter != null)
            {
                rmsFilter.OnDataReceived += HandleRMSData;
            }
        }

        private void OnDestroy()
        {
            if (rmsFilter != null)
            {
                rmsFilter.OnDataReceived -= HandleRMSData;
            }
        }

        private void HandleRMSData(float[] rmsValues, double timestamp)
        {
            // Calculate the average RMS value
            float averageRMS = 0f;
            foreach (float rms in rmsValues)
            {
                averageRMS += rms;
            }
            averageRMS /= rmsValues.Length;

            // Map the average RMS value to a speed
            float speed = Mathf.Clamp(averageRMS * maxSpeed, 0, maxSpeed);

            // Move the object forward based on the calculated speed
            if (objectToMove != null)
            {
                objectToMove.Translate(Vector3.forward * speed * Time.deltaTime);
            }
        }
    }
}