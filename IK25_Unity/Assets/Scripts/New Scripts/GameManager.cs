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

    private bool isCalibrationComplete = false; // Tracks whether calibration is complete

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
        // Hide score and last flower score during calibration
        if (scoreText != null)
        {
            scoreText.gameObject.SetActive(false);
        }

        if (lastFlowerScoreText != null)
        {
            lastFlowerScoreText.gameObject.SetActive(false);
        }
    }

    public void CompleteCalibration()
    {
        isCalibrationComplete = true;

        // Show score and last flower score after calibration
        if (scoreText != null)
        {
            scoreText.gameObject.SetActive(true);
        }

        if (lastFlowerScoreText != null)
        {
            lastFlowerScoreText.gameObject.SetActive(true);
        }
    }

    public void AddPoints(int points)
    {
        if (!isCalibrationComplete)
        {
            Debug.LogWarning("Cannot add points before calibration is complete.");
            return;
        }

        score += points;

        // Update the score in the TextMeshPro element
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }

        // Display the last collected flower's score
        if (lastFlowerScoreText != null)
        {
            lastFlowerScoreText.text = "Last Flower: +" + points;
        }

        Debug.Log("Score: " + score);
    }

    public int GetScore()
    {
        return score;
    }
}