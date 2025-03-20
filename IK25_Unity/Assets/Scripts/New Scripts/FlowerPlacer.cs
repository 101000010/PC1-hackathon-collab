using UnityEngine;

public class FlowerPlacer : MonoBehaviour
{
    [SerializeField]
    private GameObject flowerPrefab1; // First flower prefab

    [SerializeField]
    private GameObject flowerPrefab2; // Second flower prefab

    [SerializeField]
    private GameObject flowerPrefab3; // Third flower prefab

    [SerializeField]
    private GameObject flowerPrefab4; // Fourth flower prefab

    [SerializeField]
    private GameObject flowerPrefab5; // Fifth flower prefab

    [SerializeField]
    private GameObject plane; // The plane on which to place the flowers

    [SerializeField]
    private int numberOfFlowers = 10; // Number of flower objects to place

    private void Start()
    {
        if (flowerPrefab1 == null || flowerPrefab2 == null || flowerPrefab3 == null || flowerPrefab4 == null || flowerPrefab5 == null)
        {
            Debug.LogError("One or more flower prefabs are not assigned.");
            return;
        }

        if (plane == null)
        {
            Debug.LogError("Plane GameObject is not assigned.");
            return;
        }

        PlaceFlowers();
    }

    private void PlaceFlowers()
    {
        // Get the bounds of the plane
        Renderer planeRenderer = plane.GetComponent<Renderer>();
        Vector3 planeSize = planeRenderer.bounds.size;

        for (int i = 0; i < numberOfFlowers; i++)
        {
            // Randomly select a flower prefab
            GameObject flowerPrefab = GetRandomFlowerPrefab();

            // Randomly position the flower on the plane
            float randomX = Random.Range(-planeSize.x / 2, planeSize.x / 2);
            float randomZ = Random.Range(-planeSize.z / 2, planeSize.z / 2);
            Vector3 randomPosition = new Vector3(randomX, 0, randomZ) + plane.transform.position;

            // Instantiate the flower prefab at the random position
            Instantiate(flowerPrefab, randomPosition, Quaternion.identity);
        }
    }

    private GameObject GetRandomFlowerPrefab()
    {
        int randomIndex = Random.Range(0, 5);
        switch (randomIndex)
        {
            case 0:
                return flowerPrefab1;
            case 1:
                return flowerPrefab2;
            case 2:
                return flowerPrefab3;
            case 3:
                return flowerPrefab4;
            case 4:
                return flowerPrefab5;
            default:
                return flowerPrefab1; // Fallback in case of an unexpected index
        }
    }
}