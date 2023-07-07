using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class ProbeLauncher : Tool
{
    public List<GameObject> spheres = new List<GameObject>();
    public float ringLineWidth = 0.1f;
    public Material ringMaterial;
    public int ringSegments = 100;
    public float ringWidth = 0.1f;
    public string ringNamePrefix = "Intersection Ring ";
    public LayerMask ringLayerMask; // Layer selection using LayerMask

    public UnityEvent<GameObject> OnNewProbeIndicatorCreated;

    private Dictionary<string, GameObject> ringObjects = new Dictionary<string, GameObject>();

    public GameObject projectilePrefab;
    public DamageSource damageSource;

    public float fireForce = 20f;

    protected override void FireReady() {
        GameObject proj = Instantiate(projectilePrefab, firePoint.position, (null !=  lookPoint) ? lookPoint.rotation : firePoint.rotation);
        proj.GetComponent<Projectile>().owners = damageSource.owners;
        proj.GetComponent<Projectile>().damage = damageSource.DealDamage();
        proj.GetComponent<Projectile>().damageType = damageSource.damageType;
        proj.GetComponent<Projectile>().headMultiplier = damageSource.headMultiplier;
        proj.GetComponent<Rigidbody>().AddForce(((null !=  lookPoint) ? lookPoint.forward : firePoint.forward) * fireForce, ForceMode.Impulse);
    }

    protected override void StartRoutine()
    {
        base.StartRoutine();
        
        OnNewProbeIndicatorCreated.AddListener(AddSphere);
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
        List<string> keys = new List<string>(ringObjects.Keys);

        // For each pair of spheres
        for (int i = 0; i < spheres.Count; i++)
        {
            for (int j = i + 1; j < spheres.Count; j++)
            {
                ProcessSpherePair(i, j, keys);
            }
        }

        // Destroy the rings of the pairs of spheres that no longer exist
        DestroyRemainingRings(keys);
    }

    private void ProcessSpherePair(int i, int j, List<string> keys)
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

    private void CreateOrAdjustRing(int i, int j, string key, float radius1, float radius2, Vector3 center1, Vector3 center2, Vector3 centerDiff, float distance)
    {
        if (!ringObjects.ContainsKey(key))
        {
            ringObjects.Add(key, CreateNewRing(i, j, key));
        }

        AdjustRing(i, j, key, radius1, radius2, center1, center2, centerDiff, distance);
    }

    private GameObject CreateNewRing(int i, int j, string key)
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

    private void AdjustRing(int i, int j, string key, float radius1, float radius2, Vector3 center1, Vector3 center2, Vector3 centerDiff, float distance)
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

    private void DestroyRemainingRings(List<string> keys)
    {
        foreach (string key in keys)
        {
            Destroy(ringObjects[key]);
            ringObjects.Remove(key);
        }
    }

    // Converts LayerMask to a single layer index
    private int LayerMaskToLayer(LayerMask layerMask)
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
