using UnityEngine;
using System.Collections.Generic;

public class DamageZoneSpawner : MonoBehaviour {
    public GameObject damageZonePrefab;
    public List<Transform> spawnPositions;
    public int zoneCount = 3;

    private int lastSpawnIndex = -1;
    private int activeZoneCount = 0;

    private void Start() {
        // DamageZone.OnDespawn += OnZoneDespawned;

        for (int i = 0; i < zoneCount; i++) {
            SpawnZone();
        }
    }

    private void OnDestroy() {
        // DamageZone.OnDespawn -= OnZoneDespawned;
    }

    private void OnZoneDespawned() {
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
        Instantiate(damageZonePrefab, position, Quaternion.identity);

        lastSpawnIndex = index;
        activeZoneCount++;
    }
}
