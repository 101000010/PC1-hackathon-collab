using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    [SerializeField]
    private float timeInMinutes = 1f; // Time input in minutes

    [SerializeField]
    private TextMeshProUGUI timerText; // TextMeshPro element to display the timer

    [SerializeField]
    private GameObject gameOverCanvas; // Canvas GameObject to display the "Game Over" message

    [SerializeField]
    private Calibration calibration; // Reference to the Calibration script

    private float timeRemaining; // Time remaining in seconds
    private bool isTimerRunning = false; // Tracks whether the timer is running
    private bool isGameOver = false; // Tracks whether the game is over

    private void Start()
    {
        // Convert minutes to seconds but do not start the timer yet
        timeRemaining = timeInMinutes * 60f;
        UpdateTimerDisplay(); // Ensure the timer text is displayed even before starting

        // Hide the "Game Over" canvas at the start
        if (gameOverCanvas != null)
        {
            gameOverCanvas.SetActive(false);
        }

        if (calibration == null)
        {
            Debug.LogError("Calibration script is not assigned in the Inspector.");
        }
    }

    private void Update()
    {
        if (isTimerRunning)
        {
            Debug.Log("Timer is running...");
            if (timeRemaining > 0)
            {
                // Decrease the remaining time
                timeRemaining -= Time.deltaTime;

                // Update the timer display
                UpdateTimerDisplay();
            }
            else
            {
                // Stop the timer when it reaches zero
                timeRemaining = 0;
                isTimerRunning = false;
                UpdateTimerDisplay();
                EndGame();
            }
        }

        // Check for restart input if the game is over
        if (isGameOver && Input.GetKeyDown(KeyCode.Return))
        {
            RestartGame();
        }
    }

    public void StartTimer()
    {
        if (calibration != null && calibration.IsCalibrated)
        {
            // Start the timer if calibration is complete
            isTimerRunning = true;
            Debug.Log("Timer started!");
        }
        else
        {
            Debug.Log("Timer cannot start. Calibration is not complete.");
        }
    }

    private void UpdateTimerDisplay()
    {
        if (timerText != null)
        {
            // Format the time as MM:SS
            int minutes = Mathf.FloorToInt(timeRemaining / 60);
            int seconds = Mathf.FloorToInt(timeRemaining % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    private void EndGame()
    {
        isGameOver = true;

        // Activate the "Game Over" canvas
        if (gameOverCanvas != null)
        {
            gameOverCanvas.SetActive(true);
        }

        // Disable the player GameObject
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            player.SetActive(false);
        }

        Debug.Log("Game Over! Timer has finished.");
    }

    private void RestartGame()
    {
        // Reload the current scene to restart the game
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}