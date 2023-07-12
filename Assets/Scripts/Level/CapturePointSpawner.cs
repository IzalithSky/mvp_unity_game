using UnityEngine;
using System;
using System.Collections.Generic;

public class CapturePointSpawner : MonoBehaviour {
    public GameObject zonePrefab;
    public List<Transform> spawnPositions;
    public int zoneCount = 3;
    public int zonesCapturedTotal = 0;
    public int zonesCapturedMax = 10;

    public enum SpawnMode { FixedPositions, RandomTerrain }
    public SpawnMode mode;
    public Terrain terrain;
    public float xMargin = 101f;
    public float zMargin = 101f;

    private int lastSpawnIndex = -1;
    private int activeZoneCount = 0;

    public Guid spawnerID; // unique ID of this spawner

    private void Start() {
        spawnerID = Guid.NewGuid();
        CaptureZone.OnCapture += OnZoneCaptured;

        for (int i = 0; i < zoneCount; i++) {
            SpawnZone();
        }
    }

    private void OnDestroy() {
        CaptureZone.OnCapture -= OnZoneCaptured;
    }

    private void OnZoneCaptured(Guid zoneSpawnerID) {
        if (zoneSpawnerID == spawnerID) {
            activeZoneCount--;
            if (activeZoneCount < zoneCount) {
                SpawnZone();
            }
            zonesCapturedTotal++;
        }
    }

    private void SpawnZone() {
        Vector3 position = Vector3.zero;
        switch(mode)
        {
            case SpawnMode.FixedPositions:
                int index;
                do {
                    index = UnityEngine.Random.Range(0, spawnPositions.Count);
                } while (index == lastSpawnIndex);
                position = spawnPositions[index].position;
                lastSpawnIndex = index;
                break;

            case SpawnMode.RandomTerrain:
                float x = UnityEngine.Random.Range(xMargin, terrain.terrainData.size.x - xMargin);
                float z = UnityEngine.Random.Range(zMargin, terrain.terrainData.size.z - zMargin);
                float y = terrain.SampleHeight(new Vector3(x, 0, z));
                position = new Vector3(x, y, z);
                break;
        }

        GameObject zone = Instantiate(zonePrefab, position, Quaternion.identity);
        CaptureZone cz = zone.GetComponent<CaptureZone>();
        if (cz) {
            cz.spawnerID = spawnerID;
        }
        activeZoneCount++;
    }
}
