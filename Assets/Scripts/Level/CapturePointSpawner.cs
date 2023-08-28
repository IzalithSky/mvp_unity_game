using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.AI;

public class CapturePointSpawner : MonoBehaviour {
    public GameObject zonePrefab;
    public List<Transform> spawnPositions;
    public int zoneCount = 3;
    public int zonesCapturedTotal = 0;
    public int zonesCapturedMax = 10;

    public enum SpawnMode { FixedPositions, RandomTerrain, RandomNavMesh }
    public SpawnMode mode;
    public Terrain terrain;
    public float xMargin = 101f;
    public float zMargin = 101f;
    
    public float navMeshSampleRadius = 100f;
    public float navMeshMaxDistance = 15f;  

    int lastSpawnIndex = -1;
    int activeZoneCount = 0;

    public Guid spawnerID; // unique ID of this spawner

    void Start() {
        spawnerID = Guid.NewGuid();
        CaptureZone.OnCapture += OnZoneCaptured;

        for (int i = 0; i < zoneCount; i++) {
            SpawnZone();
        }
    }

    void OnDestroy() {
        CaptureZone.OnCapture -= OnZoneCaptured;
    }

    void OnZoneCaptured(Guid zoneSpawnerID) {
        if (zoneSpawnerID == spawnerID) {
            activeZoneCount--;
            if (activeZoneCount < zoneCount) {
                SpawnZone();
            }
            zonesCapturedTotal++;
        }
    }

    void SpawnZone() {
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

            case SpawnMode.RandomNavMesh:
                position = GetRandomNavMeshLocation();
                break;
        }

        GameObject zone = Instantiate(zonePrefab, position, Quaternion.identity);
        CaptureZone cz = zone.GetComponent<CaptureZone>();
        if (cz) {
            cz.spawnerID = spawnerID;
        }
        activeZoneCount++;
    }

    Vector3 GetRandomNavMeshLocation() {
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * navMeshSampleRadius;
        randomDirection += transform.position;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randomDirection, out navHit, navMeshMaxDistance, -1);
        return navHit.position;
    }
}
