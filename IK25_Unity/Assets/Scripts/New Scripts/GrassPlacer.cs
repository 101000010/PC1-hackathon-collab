using UnityEngine;

public class GrassPlacer : MonoBehaviour
{
    [SerializeField]
    private GameObject grassPrefab1; // First grass prefab

    [SerializeField]
    private GameObject grassPrefab2; // Second grass prefab

    [SerializeField]
    private GameObject grassPrefab3; // Third grass prefab

    [SerializeField]
    private GameObject grassPrefab4; // Fourth grass prefab

    [SerializeField]
    private GameObject grassPrefab5; // Fifth grass prefab

    [SerializeField]
    private GameObject grassPrefab6; // Sixth grass prefab

    [SerializeField]
    private GameObject plane; // The plane on which to place the grass

    [SerializeField]
    private int numberOfGrass = 10; // Number of grass objects to place

    private void Start()
    {
        if (grassPrefab1 == null || grassPrefab2 == null || grassPrefab3 == null || grassPrefab4 == null || grassPrefab5 == null || grassPrefab6 == null)
        {
            Debug.LogError("One or more grass prefabs are not assigned.");
            return;
        }

        if (plane == null)
        {
            Debug.LogError("Plane GameObject is not assigned.");
            return;
        }

        PlaceGrass();
    }

    private void PlaceGrass()
    {
        // Get the bounds of the plane
        Renderer planeRenderer = plane.GetComponent<Renderer>();
        Vector3 planeSize = planeRenderer.bounds.size;

        for (int i = 0; i < numberOfGrass; i++)
        {
            // Randomly select a grass prefab
            GameObject grassPrefab = GetRandomGrassPrefab();

            // Randomly position the grass on the plane
            float randomX = Random.Range(-planeSize.x / 2, planeSize.x / 2);
            float randomZ = Random.Range(-planeSize.z / 2, planeSize.z / 2);
            Vector3 randomPosition = new Vector3(randomX, 0, randomZ) + plane.transform.position;

            // Instantiate the grass prefab at the random position
            Instantiate(grassPrefab, randomPosition, Quaternion.identity);
        }
    }

    private GameObject GetRandomGrassPrefab()
    {
        int randomIndex = Random.Range(0, 6);
        switch (randomIndex)
        {
            case 0:
                return grassPrefab1;
            case 1:
                return grassPrefab2;
            case 2:
                return grassPrefab3;
            case 3:
                return grassPrefab4;
            case 4:
                return grassPrefab5;
            case 5:
                return grassPrefab6;
            default:
                return grassPrefab1; // Fallback in case of an unexpected index
        }
    }
}