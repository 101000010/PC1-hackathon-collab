using UnityEngine;
using System.Collections.Generic;
using PlayBionic.MyoOsu.Management;

public class Calibration : MonoBehaviour
{
    [SerializeField]
    private RMSFilter leftArmRMSFilter; // RMSFilter for the left arm Myo Armband

    [SerializeField]
    private RMSFilter rightArmRMSFilter; // RMSFilter for the right arm Myo Armband

    private float[] leftArmMaxValues = new float[8];
    private float[] leftArmMinValues = new float[8];

    private float[] rightArmMaxValues = new float[8];
    private float[] rightArmMinValues = new float[8];

    private bool isCalibrating = false;
    public bool IsCalibrated { get; private set; } = false; // Indicates if calibration is complete

    void Start()
    {
        // Initialize min and max values
        for (int i = 0; i < 8; i++)
        {
            leftArmMaxValues[i] = float.MinValue;
            leftArmMinValues[i] = float.MaxValue;

            rightArmMaxValues[i] = float.MinValue;
            rightArmMinValues[i] = float.MaxValue;
        }

        if (leftArmRMSFilter != null)
        {
            leftArmRMSFilter.OnDataReceived += HandleLeftArmData;
        }

        if (rightArmRMSFilter != null)
        {
            rightArmRMSFilter.OnDataReceived += HandleRightArmData;
        }
    }

    private void Update()
    {
        // Start calibration when the Spacebar is pressed
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!isCalibrating)
            {
                StartCalibration();
            }
            else
            {
                StopCalibration();
            }
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
        if (!isCalibrating) return;

        for (int i = 0; i < rmsValues.Length; i++)
        {
            leftArmMaxValues[i] = Mathf.Max(leftArmMaxValues[i], rmsValues[i]);
            leftArmMinValues[i] = Mathf.Min(leftArmMinValues[i], rmsValues[i]);
        }
    }

    private void HandleRightArmData(float[] rmsValues, double timestamp)
    {
        if (!isCalibrating) return;

        for (int i = 0; i < rmsValues.Length; i++)
        {
            rightArmMaxValues[i] = Mathf.Max(rightArmMaxValues[i], rmsValues[i]);
            rightArmMinValues[i] = Mathf.Min(rightArmMinValues[i], rmsValues[i]);
        }
    }

    public void StartCalibration()
    {
        isCalibrating = true;
        IsCalibrated = false;

        // Reset min and max values
        for (int i = 0; i < 8; i++)
        {
            leftArmMaxValues[i] = float.MinValue;
            leftArmMinValues[i] = float.MaxValue;

            rightArmMaxValues[i] = float.MinValue;
            rightArmMinValues[i] = float.MaxValue;
        }

        Debug.Log("Calibration started. Move your muscles to their extremes.");
    }

    public void StopCalibration()
    {
        isCalibrating = false;
        IsCalibrated = true;

        Debug.Log("Calibration completed.");
        Debug.Log("Left Arm Max Values: " + string.Join(", ", leftArmMaxValues));
        Debug.Log("Left Arm Min Values: " + string.Join(", ", leftArmMinValues));
        Debug.Log("Right Arm Max Values: " + string.Join(", ", rightArmMaxValues));
        Debug.Log("Right Arm Min Values: " + string.Join(", ", rightArmMinValues));
    }

    public float[] NormalizeLeftArmData(float[] rmsValues)
    {
        float[] normalizedValues = new float[8];
        for (int i = 0; i < rmsValues.Length; i++)
        {
            normalizedValues[i] = Mathf.InverseLerp(leftArmMinValues[i], leftArmMaxValues[i], rmsValues[i]);
        }
        return normalizedValues;
    }

    public float[] NormalizeRightArmData(float[] rmsValues)
    {
        float[] normalizedValues = new float[8];
        for (int i = 0; i < rmsValues.Length; i++)
        {
            normalizedValues[i] = Mathf.InverseLerp(rightArmMinValues[i], rightArmMaxValues[i], rmsValues[i]);
        }
        return normalizedValues;
    }
}