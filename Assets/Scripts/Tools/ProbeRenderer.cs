using System.Collections.Generic;
using UnityEngine;

public class ProbeRenderer : MonoBehaviour
{    
    public ProbeTracker probeTracker;
    public Material ringMaterial;
    public int ringSegments = 100;
    public float ringWidth = 0.1f;
    public LayerMask ringLayerMask;

    private string ringNamePrefix = "Intersection Ring ";
    private Dictionary<string, GameObject> ringObjects = new Dictionary<string, GameObject>();
    private HashSet<string> currentProcessedPairs = new HashSet<string>();

    
    void Update()
    {
        if (!probeTracker.targetGameObject) {
            GameObject target = GameObject.FindWithTag("EAnomaly");
            if (target) {
                probeTracker.targetGameObject = target;
            }
        }

        // Reset the set of processed pairs for this frame
        currentProcessedPairs.Clear();

        // For each pair of spheres
        for (int i = 0; i < probeTracker.probes.Count; i++) 
        {
            for (int j = i + 1; j < probeTracker.probes.Count; j++) 
            {
                ProcessSpherePair(i, j);
            }
        }

        CleanupUnusedRings();
    }

    void ProcessSpherePair(int i, int j)
    {
        string key = i.ToString() + "_" + j.ToString();

        // Add the pair to the current set of processed pairs
        currentProcessedPairs.Add(key);

        float radius1 = probeTracker.probes[i].distance;
        float radius2 = probeTracker.probes[j].distance;
        Vector3 center1 = probeTracker.probes[i].transform.position;
        Vector3 center2 = probeTracker.probes[j].transform.position;
        Vector3 centerDiff = center2 - center1;
        float distance = centerDiff.magnitude;

        // Check if the spheres are intersecting
        if (distance < radius1 + radius2 && distance > Mathf.Abs(radius1 - radius2))
        {
            CreateOrAdjustRing(i, j, key, radius1, radius2, center1, center2, centerDiff, distance);
        }
        else if (ringObjects.ContainsKey(key))
        {
            // If the spheres are no longer intersecting, destroy the corresponding ring
            Destroy(ringObjects[key]);
            ringObjects.Remove(key);
        }
    }

    void CreateOrAdjustRing(int i, int j, string key, float radius1, float radius2, Vector3 center1, Vector3 center2, Vector3 centerDiff, float distance)
    {
        if (!ringObjects.ContainsKey(key))
        {
            ringObjects.Add(key, CreateNewRing(i, j, key));
        }

        AdjustRing(i, j, key, radius1, radius2, center1, center2, centerDiff, distance);
    }

    GameObject CreateNewRing(int i, int j, string key)
    {
        // Create a new ring
        GameObject ringObject = new GameObject(ringNamePrefix + key);
        LineRenderer newRing = ringObject.AddComponent<LineRenderer>();
        newRing.loop = true;
        newRing.positionCount = ringSegments;
        newRing.startWidth = ringWidth;
        newRing.endWidth = ringWidth;
        newRing.material = ringMaterial;
        newRing.gameObject.layer = LayerMaskToLayer(ringLayerMask); // Assign the desired layer to the ring object

        return ringObject;
    }

    void AdjustRing(int i, int j, string key, float radius1, float radius2, Vector3 center1, Vector3 center2, Vector3 centerDiff, float distance)
    {
        // Calculate the center and radius of the intersection circle
        Vector3 circleCenter = center1 + ((radius1 * radius1 - radius2 * radius2 + distance * distance) / (2 * distance)) * (centerDiff / distance);
        float circleRadius = Mathf.Sqrt(radius1 * radius1 - (circleCenter - center1).magnitude * (circleCenter - center1).magnitude);

        // Calculate the points around the intersection circle and assign them to the LineRenderer
        LineRenderer currentRing = ringObjects[key].GetComponent<LineRenderer>();
        Vector3 forward = (centerDiff / distance).normalized;
        Vector3 right = Vector3.Cross(forward, Vector3.up).normalized;
        Vector3 up = Vector3.Cross(right, forward).normalized;

        for (int k = 0; k < currentRing.positionCount; k++)
        {
            float angle = k * 2 * Mathf.PI / currentRing.positionCount;
            Vector3 direction = (Mathf.Cos(angle) * right + Mathf.Sin(angle) * up).normalized;
            currentRing.SetPosition(k, circleCenter + direction * circleRadius);
        }
    }

    void CleanupUnusedRings()
    {
        List<string> keysToRemove = new List<string>();
        foreach (var key in ringObjects.Keys)
        {
            if (!currentProcessedPairs.Contains(key))
            {
                Destroy(ringObjects[key]);
                keysToRemove.Add(key);
            }
        }
        foreach (var key in keysToRemove)
        {
            ringObjects.Remove(key);
        }
    }

    // Converts LayerMask to a single layer index
    int LayerMaskToLayer(LayerMask layerMask)
    {
        int layerNumber = layerMask.value;
        int layer = 0;
        while (layerNumber > 0)
        {
            layerNumber >>= 1;
            layer++;
        }
        return layer - 1;
    }
}
