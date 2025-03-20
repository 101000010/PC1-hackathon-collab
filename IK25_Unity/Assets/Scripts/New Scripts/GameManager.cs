using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private int score;

    [SerializeField]
    private TextMeshProUGUI scoreText; // TextMeshPro element to display the score

    [SerializeField]
    private TextMeshProUGUI lastFlowerScoreText; // TextMeshPro element to display the last collected flower's score

    [SerializeField]
    private GameObject canvas; // Canvas GameObject to activate after calibration

    [SerializeField]
    private Calibration calibration; // Reference to the Calibration script

    private void Awake()
    {
        // Ensure that there is only one instance of GameManager
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Hide score, last flower score, and canvas during calibration
        if (scoreText != null)
        {
            scoreText.gameObject.SetActive(false);
        }

        if (lastFlowerScoreText != null)
        {
            lastFlowerScoreText.gameObject.SetActive(false);
        }

        if (canvas != null)
        {
            canvas.SetActive(false);
        }

        // Check if the Calibration script is assigned
        if (calibration == null)
        {
            Debug.LogError("Calibration script is not assigned in the Inspector.");
        }
    }

    private void Update()
    {
        // Check if calibration is complete and activate the canvas
        if (calibration != null && calibration.IsCalibrated)
        {
            CompleteCalibration();
        }
    }

    public void CompleteCalibration()
    {
        // Show score and last flower score after calibration
        if (scoreText != null)
        {
            scoreText.gameObject.SetActive(true);
        }

        if (lastFlowerScoreText != null)
        {
            lastFlowerScoreText.gameObject.SetActive(true);
        }

        // Activate the canvas
        if (canvas != null)
        {
            canvas.SetActive(true);
            Debug.Log("Canvas activated.");
        }
        else
        {
            Debug.LogError("Canvas is not assigned in the Inspector.");
        }
    }

    public void AddPoints(int points)
    {
        if (calibration == null || !calibration.IsCalibrated)
        {
            Debug.LogWarning("Cannot add points before calibration is complete.");
            return;
        }

        score += points;

        // Update the score in the TextMeshPro element
        if (scoreText != null)
        {
            scoreText.text = score.ToString(); // Convert int to string
        }

        // Display the last collected flower's score
        if (lastFlowerScoreText != null)
        {
            lastFlowerScoreText.text = points.ToString(); // Convert int to string
        }

        Debug.Log("Score: " + score);
    }

    public int GetScore()
    {
        return score;
    }
}