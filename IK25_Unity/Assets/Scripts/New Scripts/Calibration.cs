using UnityEngine;
using System.Collections;
using PlayBionic.MyoOsu.Management; // Replace with the actual namespace of RMSFilter

public class Calibration : MonoBehaviour
{
    [SerializeField]
    private RMSFilter leftArmRMSFilter; // RMSFilter for the left arm Myo Armband

    [SerializeField]
    private RMSFilter rightArmRMSFilter; // RMSFilter for the right arm Myo Armband

    private float[] leftArmMaxValues = new float[8];
    private float[] rightArmMaxValues = new float[8];

    private int rightArmUpChannel = -1; // Channel for right arm up movement
    private int rightArmDownChannel = -1; // Channel for right arm down movement
    private int leftArmLeftChannel = -1; // Channel for left arm left movement
    private int leftArmRightChannel = -1; // Channel for left arm right movement

    private bool isCalibrating = false;
    public bool IsCalibrated { get; private set; } = false; // Indicates if calibration is complete

    private Coroutine calibrationCoroutine;

    [SerializeField]
    private float calibrationDuration = 5f; // Adjustable duration for each calibration step

    void Start()
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

    private void Update()
    {
        // Start calibration when the Spacebar is pressed
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!isCalibrating)
            {
                StartCalibration();
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
        }
    }

    private void HandleRightArmData(float[] rmsValues, double timestamp)
    {
        if (!isCalibrating) return;

        for (int i = 0; i < rmsValues.Length; i++)
        {
            rightArmMaxValues[i] = Mathf.Max(rightArmMaxValues[i], rmsValues[i]);
        }
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

    public void StartCalibration()
    {
        if (isCalibrating) return;

        isCalibrating = true;
        IsCalibrated = false;

        // Reset max values
        ResetMaxValues();

        // Start the calibration process
        calibrationCoroutine = StartCoroutine(CalibrationProcess());
    }

    private IEnumerator CalibrationProcess()
    {
        Debug.Log("Move your right arm up and contract your muscles.");
        yield return new WaitForSeconds(calibrationDuration); // Wait for the specified duration
        rightArmUpChannel = GetPrimaryChannel(rightArmMaxValues);
        Debug.Log($"Right arm up channel: {rightArmUpChannel}");

        ResetMaxValues();
        Debug.Log("Move your right arm down and contract your muscles.");
        yield return new WaitForSeconds(calibrationDuration); // Wait for the specified duration
        rightArmDownChannel = GetPrimaryChannel(rightArmMaxValues);
        Debug.Log($"Right arm down channel: {rightArmDownChannel}");

        // Check if the right arm indices are the same
        if (rightArmUpChannel == rightArmDownChannel)
        {
            Debug.LogError("Calibration failed, please try again.");
            Debug.Log("Restarting the Calibration...");
            isCalibrating = false; // Reset the flag before restarting
            yield return new WaitForSeconds(4f); // Wait for 4 seconds before restarting
            StartCalibration();
            yield break;
        }

        ResetMaxValues();
        Debug.Log("Move your left arm to the left and contract your muscles.");
        yield return new WaitForSeconds(calibrationDuration); // Wait for the specified duration
        leftArmLeftChannel = GetPrimaryChannel(leftArmMaxValues);
        Debug.Log($"Left arm left channel: {leftArmLeftChannel}");

        ResetMaxValues();
        Debug.Log("Move your left arm to the right and contract your muscles.");
        yield return new WaitForSeconds(calibrationDuration); // Wait for the specified duration
        leftArmRightChannel = GetPrimaryChannel(leftArmMaxValues);
        Debug.Log($"Left arm right channel: {leftArmRightChannel}");

        // Check if the left arm indices are the same
        if (leftArmLeftChannel == leftArmRightChannel)
        {
            Debug.LogError("Calibration failed, please try again.");
            Debug.Log("Restarting the Calibration...");
            isCalibrating = false; // Reset the flag before restarting
            yield return new WaitForSeconds(4f); // Wait for 4 seconds before restarting
            StartCalibration();
            yield break;
        }

        Debug.Log("Calibration completed.");
        isCalibrating = false;
        IsCalibrated = true;
    }

    private void ResetMaxValues()
    {
        for (int i = 0; i < 8; i++)
        {
            leftArmMaxValues[i] = float.MinValue;
            rightArmMaxValues[i] = float.MinValue;
        }
    }

    public int GetRightArmUpChannel()
    {
        return rightArmUpChannel;
    }

    public int GetRightArmDownChannel()
    {
        return rightArmDownChannel;
    }

    public int GetLeftArmLeftChannel()
    {
        return leftArmLeftChannel;
    }

    public int GetLeftArmRightChannel()
    {
        return leftArmRightChannel;
    }
}