using System.Collections.Generic;
using UnityEngine;

public class SpawnerController : MonoBehaviour {
    public List<Spawner> spawners; // List of Spawner objects
    public CapturePointSpawner capturePointSpawner; // CapturePointSpawner

    private void Update() {
        // Check if zonesCapturedTotal has reached the limit
        if (capturePointSpawner.zonesCapturedTotal >= capturePointSpawner.zonesCapturedMax) {
            // If it has, disable automaticSpawning for all Spawner objects
            foreach (Spawner spawner in spawners) {
                spawner.automaticSpawning = false;
            }
        }
    }
}
