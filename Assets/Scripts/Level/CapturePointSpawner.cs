using UnityEngine;
using System.Collections.Generic;

public class CapturePointSpawner : MonoBehaviour {
    public GameObject zonePrefab;
    public List<Transform> spawnPositions;
    public int zoneCount = 3;

    private int lastSpawnIndex = -1;
    private int activeZoneCount = 0;

    private void Start() {
        CaptureZone.OnCapture += OnZoneCaptured;

        for (int i = 0; i < zoneCount; i++) {
            SpawnZone();
        }
    }

    private void OnDestroy() {
        CaptureZone.OnCapture -= OnZoneCaptured;
    }

    private void OnZoneCaptured() {
        activeZoneCount--;
        if (activeZoneCount < zoneCount) {
            SpawnZone();
        }
    }

    private void SpawnZone() {
        int index;
        do {
            index = Random.Range(0, spawnPositions.Count);
        } while (index == lastSpawnIndex);

        Vector3 position = spawnPositions[index].position;
        Instantiate(zonePrefab, position, Quaternion.identity);

        lastSpawnIndex = index;
        activeZoneCount++;
    }
}
