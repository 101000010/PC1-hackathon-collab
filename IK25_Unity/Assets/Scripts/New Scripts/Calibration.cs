using UnityEngine;
using UnityEngine.UI; // For UI Text
using System.Collections;
using PlayBionic.MyoOsu.Management; // Replace with the actual namespace of RMSFilter
using TMPro; // For TextMeshProUGUI

public class Calibration : MonoBehaviour
{
    [SerializeField]
    private RMSFilter leftArmRMSFilter; // RMSFilter for the left arm Myo Armband

    [SerializeField]
    private RMSFilter rightArmRMSFilter; // RMSFilter for the right arm Myo Armband

    [SerializeField]
    private TextMeshProUGUI calibrationMessage; // Reference to the Text UI element

    private float[] leftArmMaxValues = new float[8];
    private float[] rightArmMaxValues = new float[8];

    private int rightArmUpChannel = -1; // Channel for right arm up movement
    private int rightArmDownChannel = -1; // Channel for right arm down movement
    private int leftArmLeftChannel = -1; // Channel for left arm left movement
    private int leftArmRightChannel = -1; // Channel for left arm right movement

    private bool isCalibrating = false;
    private bool calibrationFailed = false; // Flag to track if calibration failed
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

        // Clear the message at the start
        UpdateCalibrationMessage("");
    }

    private void Update()
    {
        // Allow manual restart if calibration failed
        if (calibrationFailed && Input.GetKeyDown(KeyCode.Space))
        {
            UpdateCalibrationMessage("Restarting the Calibration...");
            calibrationFailed = false; // Reset the failure flag
            StartCalibration();
        }

        // Start calibration when the Spacebar is pressed and calibration is not in progress
        if (!isCalibrating && !calibrationFailed && Input.GetKeyDown(KeyCode.Space))
        {
            StartCalibration();
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

        // Debug log to verify data
        //Debug.Log($"HandleLeftArmData: Updated leftArmMaxValues: {string.Join(", ", leftArmMaxValues)}");
    }

    private void HandleRightArmData(float[] rmsValues, double timestamp)
    {
        if (!isCalibrating) return;

        for (int i = 0; i < rmsValues.Length; i++)
        {
            rightArmMaxValues[i] = Mathf.Max(rightArmMaxValues[i], rmsValues[i]);
        }

        // Debug log to verify data
        //Debug.Log($"HandleRightArmData: Updated rightArmMaxValues: {string.Join(", ", rightArmMaxValues)}");
    }

    private int GetPrimaryChannel(float[] maxValues)
    {
        int primaryChannel = -1; // Default to an invalid index
        float highestValue = float.MinValue;

        for (int i = 0; i < maxValues.Length; i++)
        {
            if (maxValues[i] > highestValue)
            {
                highestValue = maxValues[i];
                primaryChannel = i;
            }
        }

        // Validate the index
        if (primaryChannel < 0 || primaryChannel >= maxValues.Length || highestValue == float.MinValue)
        {
            //Debug.LogError("GetPrimaryChannel: Invalid primary channel index or no valid data in maxValues.");
        }

        return primaryChannel;
    }

    private int GetSecondHighestChannel(float[] maxValues, int excludeChannel)
    {
        int secondHighestChannel = -1; // Default to an invalid index
        float secondHighestValue = float.MinValue;

        for (int i = 0; i < maxValues.Length; i++)
        {
            if (i == excludeChannel) continue; // Skip the primary channel

            if (maxValues[i] > secondHighestValue)
            {
                secondHighestValue = maxValues[i];
                secondHighestChannel = i;
            }
        }

        // Validate the index
        if (secondHighestChannel < 0 || secondHighestChannel >= maxValues.Length)
        {
            //Debug.LogError("GetSecondHighestChannel: Invalid second highest channel index.");
        }

        return secondHighestChannel;
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
        // Right arm up calibration
        yield return StartCoroutine(DisplayCountdownMessage("Move your right Hand up and hold it there.", calibrationDuration));
        rightArmUpChannel = GetPrimaryChannel(rightArmMaxValues);

        if (rightArmUpChannel < 0 || rightArmUpChannel >= rightArmMaxValues.Length)
        {
            UpdateCalibrationMessage("Calibration failed: Invalid right arm up channel.");
            calibrationFailed = true;
            isCalibrating = false;
            yield break;
        }

        Debug.Log($"Right arm up channel: {rightArmUpChannel}");

        // Reset and proceed to the next step
        ResetMaxValues();

        // Right arm down calibration
        yield return StartCoroutine(DisplayCountdownMessage("Move your right Hand down and hold it there.", calibrationDuration));
        rightArmDownChannel = GetPrimaryChannel(rightArmMaxValues);

        if (rightArmDownChannel < 0 || rightArmDownChannel >= rightArmMaxValues.Length)
        {
            UpdateCalibrationMessage("Calibration failed: Invalid right arm down channel.");
            calibrationFailed = true;
            isCalibrating = false;
            yield break;
        }

        // Ensure the second channel is distinct
        if (rightArmDownChannel == rightArmUpChannel)
        {
            rightArmDownChannel = GetSecondHighestChannel(rightArmMaxValues, rightArmUpChannel);
        }

        if (rightArmDownChannel < 0 || rightArmDownChannel >= rightArmMaxValues.Length)
        {
            UpdateCalibrationMessage("Calibration failed: Invalid right arm down channel after adjustment.");
            calibrationFailed = true;
            isCalibrating = false;
            yield break;
        }

        Debug.Log($"Right arm down channel: {rightArmDownChannel}");

        // Reset and proceed to the next step
        ResetMaxValues();

        // Left arm left calibration
        yield return StartCoroutine(DisplayCountdownMessage("Move your left Hand to the left and hold it there.", calibrationDuration));
        leftArmLeftChannel = GetPrimaryChannel(leftArmMaxValues);

        if (leftArmLeftChannel < 0 || leftArmLeftChannel >= leftArmMaxValues.Length)
        {
            UpdateCalibrationMessage("Calibration failed: Invalid left arm left channel.");
            calibrationFailed = true;
            isCalibrating = false;
            yield break;
        }

        Debug.Log($"Left arm left channel: {leftArmLeftChannel}");

        // Reset and proceed to the next step
        ResetMaxValues();

        // Left arm right calibration
        yield return StartCoroutine(DisplayCountdownMessage("Move your left Hand to the right and hold it there.", calibrationDuration));
        leftArmRightChannel = GetPrimaryChannel(leftArmMaxValues);

        if (leftArmRightChannel < 0 || leftArmRightChannel >= leftArmMaxValues.Length)
        {
            UpdateCalibrationMessage("Calibration failed: Invalid left arm right channel.");
            calibrationFailed = true;
            isCalibrating = false;
            yield break;
        }

        // Ensure the second channel is distinct
        if (leftArmRightChannel == leftArmLeftChannel)
        {
            leftArmRightChannel = GetSecondHighestChannel(leftArmMaxValues, leftArmLeftChannel);
        }

        if (leftArmRightChannel < 0 || leftArmRightChannel >= leftArmMaxValues.Length)
        {
            UpdateCalibrationMessage("Calibration failed: Invalid left arm right channel after adjustment.");
            calibrationFailed = true;
            isCalibrating = false;
            yield break;
        }

        Debug.Log($"Left arm right channel: {leftArmRightChannel}");

        // Calibration completed successfully
        UpdateCalibrationMessage("Calibration completed successfully.");
        yield return new WaitForSeconds(2f); // Wait for 2 seconds before clearing the message
        UpdateCalibrationMessage(""); // Clear the message

        isCalibrating = false;
        IsCalibrated = true;
    }

    private IEnumerator DisplayCountdownMessage(string baseMessage, float duration)
    {
        float remainingTime = duration;

        while (remainingTime > 0)
        {
            UpdateCalibrationMessage($"{baseMessage} ({remainingTime:F1}s remaining)");
            yield return new WaitForSeconds(0.1f); // Update every 0.1 seconds
            remainingTime -= 0.1f;
        }

        UpdateCalibrationMessage(baseMessage); // Final message without countdown
    }

    private void ResetMaxValues()
    {
        for (int i = 0; i < 8; i++)
        {
            leftArmMaxValues[i] = 0f; // Reset to 0 instead of float.MinValue
            rightArmMaxValues[i] = 0f; // Reset to 0 instead of float.MinValue
        }

        Debug.Log("ResetMaxValues: Arrays reset to 0.");
    }

    private void UpdateCalibrationMessage(string message)
    {
        if (calibrationMessage != null)
        {
            calibrationMessage.text = message; // Update the TextMeshProUGUI text
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