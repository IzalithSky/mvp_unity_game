using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProbeRenderer : MonoBehaviour
{
    public GameObject targetGameObject;
    
    public ProbeTracker probeTracker;

    public float ringLineWidth = 0.1f;
    public Material ringMaterial;
    public int ringSegments = 100;
    public float ringWidth = 0.1f;
    public LayerMask ringLayerMask;

    public GameObject spherePrefab;


    string ringNamePrefix = "Intersection Ring ";
    List<GameObject> spheres = new List<GameObject>();
    Dictionary<string, GameObject> ringObjects = new Dictionary<string, GameObject>();


    void Awake()
    {
        GameObject target = GameObject.FindWithTag("EAnomaly");
        if (target) {
            targetGameObject = target;
        }
    }

    void Update()
    {
        List<string> keys = new List<string>(ringObjects.Keys);

        // For each pair of spheres
        for (int i = 0; i < spheres.Count; i++) {
            for (int j = i + 1; j < spheres.Count; j++) {
                ProcessSpherePair(i, j, keys);
            }
        }

        DestroyRemainingRings(keys);
    }

    void AddSphere(GameObject newSphere)
    {
        spheres.Add(newSphere);
    }

    void RemoveSphere(GameObject sphereToRemove)
    {
        spheres.Remove(sphereToRemove);

        // Destroy rings related to this sphere
        List<string> keysToRemove = new List<string>();
        foreach (var pair in ringObjects)
        {
            string[] indices = pair.Key.Split('_');
            if (indices[0] == sphereToRemove.name || indices[1] == sphereToRemove.name)
            {
                Destroy(pair.Value);
                keysToRemove.Add(pair.Key);
            }
        }

        foreach (string key in keysToRemove)
        {
            ringObjects.Remove(key);
        }
    }

    void ProcessSpherePair(int i, int j, List<string> keys)
    {
        string key = i.ToString() + "_" + j.ToString();
        keys.Remove(key);

        float radius1 = spheres[i].transform.localScale.x / 2f;
        float radius2 = spheres[j].transform.localScale.x / 2f;

        Vector3 center1 = spheres[i].transform.position;
        Vector3 center2 = spheres[j].transform.position;

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

    void DestroyRemainingRings(List<string> keys)
    {
        foreach (string key in keys)
        {
            Destroy(ringObjects[key]);
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
