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

    private int leftArmPrimaryChannel = -1; // Channel with the highest value for the left arm
    private int rightArmPrimaryChannel = -1; // Channel with the highest value for the right arm

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

        Debug.Log("Calibration started. For the right arm, move your hand up and down and contract your muscles. For the left arm, move your hand left and right and contract your muscles.");
    }

    public void StopCalibration()
    {
        isCalibrating = false;
        IsCalibrated = true;

        // Identify the primary channel for each arm
        leftArmPrimaryChannel = GetPrimaryChannel(leftArmMaxValues);
        rightArmPrimaryChannel = GetPrimaryChannel(rightArmMaxValues);

        Debug.Log("Calibration completed.");
        Debug.Log("Left Arm Max Values: " + string.Join(", ", leftArmMaxValues));
        Debug.Log("Left Arm Min Values: " + string.Join(", ", leftArmMinValues));
        Debug.Log("Right Arm Max Values: " + string.Join(", ", rightArmMaxValues));
        Debug.Log("Right Arm Min Values: " + string.Join(", ", rightArmMinValues));
        Debug.Log($"Left Arm Primary Channel: {leftArmPrimaryChannel}");
        Debug.Log($"Right Arm Primary Channel: {rightArmPrimaryChannel}");
    }

    private int GetPrimaryChannel(float[] maxValues)
    {
        int primaryChannel = 0;
        float highestValue = float.MinValue;

        for (int i = 0; i < maxValues.Length; i++)
        {
            if (maxValues[i] > highestValue)
            {
                highestValue = maxValues[i];
                primaryChannel = i;
            }
        }

        return primaryChannel;
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

    public int GetLeftArmPrimaryChannel()
    {
        return leftArmPrimaryChannel;
    }

    public int GetRightArmPrimaryChannel()
    {
        return rightArmPrimaryChannel;
    }
}