using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DamageZoneShooter : Tool
{
    public Transform target;
    public GameObject damageZonePrefab;
    public float observeTime = 0.5f;
    public float positionSearchRadius = 5f;

    Vector3 futurePosition;
    Vector3 previousPosition; // Position in the previous frame
    Vector3 velocity; // Estimated velocity of the target
    Queue<Vector3> positions; // Positions over the past N seconds
    Queue<float> timestamps; // Timestamps of the recorded positions

    Vector3 spawnPosition = Vector3.zero;

    void Start()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length > 0) {
            target = players[0].GetComponentInChildren<Collider>().transform;
        }

        futurePosition = target.position;
        previousPosition = target.position;
        positions = new Queue<Vector3>();
        timestamps = new Queue<float>();

    }

    void Update()
    {
        // Calculate current velocity
        Vector3 currentPosition = target.position;
        float currentTime = Time.time;
        velocity = (currentPosition - previousPosition) / Time.deltaTime;

        // Add current position and timestamp to the queues
        positions.Enqueue(currentPosition);
        timestamps.Enqueue(currentTime);

        // Remove old data
        while (currentTime - timestamps.Peek() > observeTime)
        {
            positions.Dequeue();
            timestamps.Dequeue();
        }

        // Predict future position based on current velocity
        futurePosition = currentPosition + velocity * observeTime;

        previousPosition = currentPosition;
    }

    protected override void FireReady() {
        if (findPositionOnNavmesh(futurePosition, out spawnPosition)) {
            Instantiate(damageZonePrefab, spawnPosition, Quaternion.identity);
        }
    }

    bool findPositionOnNavmesh(Vector3 position, out Vector3 found) {
        found = Vector3.zero;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(
            position, 
            out hit, 
            positionSearchRadius, 
            NavMesh.AllAreas))
        {
            found = hit.position;
            return true;
        } else {
            return false;
        }
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(spawnPosition, 0.1f);
    }
}
