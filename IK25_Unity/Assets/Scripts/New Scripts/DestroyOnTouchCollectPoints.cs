using UnityEngine;

public class DestroyOnTouchCollectPoints : MonoBehaviour
{
    [SerializeField]
    private GameObject bee;

    [SerializeField]
    private GameObject blossom;

    private void Start()
    {
        if (bee == null)
        {
            Debug.LogError("Bee GameObject is not assigned.");
        }
        if (blossom == null)
        {
            Debug.LogError("Blossom GameObject is not assigned.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter called with: " + other.gameObject.name);
        if (other.gameObject == bee)
        {
            Debug.Log("Collision with object detected.");
            
            // Get the color of the blossom object
            Color objectColor = blossom.GetComponent<Renderer>().material.color;
            string hexColor = ColorUtility.ToHtmlStringRGB(objectColor);
            Debug.Log("Blossom color (hex): #" + hexColor);

            // Convert the hex color to points
            int points = ConvertHexColorToPoints(hexColor);

            // Handle the points (e.g., add to score)
            AddPointsToScore(points);

            // Destroy the entire flower (this GameObject)
            Destroy(gameObject);
        }
    }

    private int ConvertHexColorToPoints(string hexColor)
    {
        // Example conversion logic based on hex color
        switch (hexColor)
        {
            case "FFEF00": // Level1
                return 5;
            case "FFB000": // Level2
                return 10;
            case "FF8700": // Level3
                return 20;
            case "FF5000": // Level4
                return 35;
            case "93000A": // Level5
                return 55;
            default:
                return 0;
        }
    }

    public void AddPointsToScore(int points)
    {
        GameManager.Instance.AddPoints(points);
    }
}