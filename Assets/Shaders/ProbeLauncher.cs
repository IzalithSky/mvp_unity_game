using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;


public class ProbeLauncher : MonoBehaviour
{
    public List<GameObject> spheres = new List<GameObject>();
    public float ringLineWidth = 0.1f;
    public Material ringMaterial;
    public int ringSegments = 100;
    public float ringWidth = 0.1f;
    public string ringNamePrefix = "Intersection Ring ";
    
    public UnityEvent<GameObject> OnNewProbeIndicatorCreated;

    private Dictionary<string, GameObject> ringObjects = new Dictionary<string, GameObject>();
    private Vector3[] circlePoints;


    private void Start()
    {
        OnNewProbeIndicatorCreated.AddListener(AddSphere);
        
        // Calculate the points of a unit circle
        circlePoints = new Vector3[ringSegments];
        for (int i = 0; i < ringSegments; i++)
        {
            float angle = i * 2 * Mathf.PI / ringSegments;
            circlePoints[i] = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe when this object is destroyed to prevent memory leaks
        OnNewProbeIndicatorCreated.RemoveListener(AddSphere);
    }

    private void AddSphere(GameObject newSphere)
    {
        spheres.Add(newSphere);
        // Subscribe to the OnDestroyed event of the ProbeIndicator
        ProbeIndicator probeIndicator = newSphere.GetComponent<ProbeIndicator>();
        if (probeIndicator != null)
        {
            probeIndicator.OnDestroyed.AddListener(RemoveSphere);
        }
    }

    private void RemoveSphere(GameObject sphereToRemove)
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

    void Update()
    {
        // For each pair of spheres
        for (int i = 0; i < spheres.Count; i++)
        {
            for (int j = i + 1; j < spheres.Count; j++)
            {
                string key = i.ToString() + "_" + j.ToString();

                float radius1 = spheres[i].transform.localScale.x / 2f;
                float radius2 = spheres[j].transform.localScale.x / 2f;

                Vector3 center1 = spheres[i].transform.position;
                Vector3 center2 = spheres[j].transform.position;

                Vector3 centerDiff = center2 - center1;
                float distance = centerDiff.magnitude;

                // Check if the spheres are intersecting
                if (distance < radius1 + radius2 && distance > Mathf.Abs(radius1 - radius2))
                {
                    if (!ringObjects.ContainsKey(key))
                    {
                        // Create a new ring
                        GameObject ringObject = new GameObject(ringNamePrefix + key);
                        LineRenderer newRing = ringObject.AddComponent<LineRenderer>();
                        newRing.loop = true;
                        newRing.positionCount = ringSegments;
                        newRing.startWidth = ringWidth;
                        newRing.endWidth = ringWidth;
                        newRing.material = ringMaterial;

                        ringObjects.Add(key, ringObject);
                    }

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
                        Vector3 direction = right * circlePoints[k].x + up * circlePoints[k].y;
                        currentRing.SetPosition(k, circleCenter + direction * circleRadius);
                    }
                }
                else if (ringObjects.ContainsKey(key))
                {
                    // If the spheres are no longer intersecting, destroy the corresponding ring
                    Destroy(ringObjects[key]);
                    ringObjects.Remove(key);
                }
            }
        }

        // Remove rings from the dictionary if their corresponding spheres have been destroyed
        List<string> keysToRemove = new List<string>();
        foreach (var pair in ringObjects)
        {
            if (pair.Value == null)
            {
                keysToRemove.Add(pair.Key);
            }
        }

        foreach (string key in keysToRemove)
        {
            ringObjects.Remove(key);
        }
    }
}
