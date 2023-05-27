using UnityEngine;

public class CapturePointSpawner : MonoBehaviour
{
    public GameObject zonePrefab; // Assign the capture zone prefab in the inspector
    public float planeSize = 50f; // The size of your plane. Adjust this according to your needs.

    private void Start()
    {
        // Subscribe to the OnCapture event
        CaptureZone.OnCapture += SpawnZone;

        // Spawn initial zone
        SpawnZone();
    }

    private void OnDestroy()
    {
        // Unsubscribe from the OnCapture event when this object is destroyed to prevent memory leaks
        CaptureZone.OnCapture -= SpawnZone;
    }

    private void SpawnZone()
    {
        // Generate a random position within the boundaries of the plane
        float x = Random.Range(-planeSize / 2, planeSize / 2);
        float z = Random.Range(-planeSize / 2, planeSize / 2);
        Vector3 randomPosition = new Vector3(x, 0, z);

        // Spawn a new zone at the random position
        Instantiate(zonePrefab, randomPosition, Quaternion.identity);
    }
}
