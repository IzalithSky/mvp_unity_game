using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : Tool {
    public GameObject tracer;
    public ParticleSystem muzzleFlash;
    public GameObject impactFlash;
    public GameObject bmark;
    public Transform tracerSource;
    public DamageSource damageSource;

    public float bmarkTtl = 20f;
    public float tracerDistance = 100f;
    public float tracerTtl = 0.05f;
    public float spreadAngle = 8f; // Spread angle in degrees

    protected override void FireReady()
    {
        muzzleFlash.Play();

        List<Vector3> rayDirs = GenerateConeDirections(firePoint.forward, spreadAngle, 8, 4);   
        foreach (Vector3 spreadDirection in rayDirs)
        {
            RaycastHit hit;

            if (Physics.Raycast(lookPoint.position, spreadDirection, out hit, Mathf.Infinity, mask))
            {
                DrawTracer(tracerSource.position, hit.point);

                GameObject impfl = Instantiate(impactFlash, hit.point + (hit.normal * .001f), Quaternion.LookRotation(hit.normal));
                Destroy(impfl, impfl.GetComponent<ParticleSystem>().main.duration);

                GameObject bm1 = Instantiate(bmark, hit.point + (hit.normal * .001f), Quaternion.FromToRotation(Vector3.up, hit.normal));
                bm1.transform.parent = hit.transform.gameObject.transform;
                Destroy(bm1, bmarkTtl);

                damageSource.TryHit(hit.collider.gameObject);
            }
            else
            {
                Vector3 tracerEnd = firePoint.position + spreadDirection * tracerDistance;
                DrawTracer(tracerSource.position, tracerEnd);
            }
        }
    }

    public List<Vector3> GenerateConeDirections(Vector3 baseDirection, float angle, int outerCount, int innerCount)
    {
        List<Vector3> directions = new List<Vector3>();

        // The center direction
        directions.Add(baseDirection);

        float outerRadius = Mathf.Tan(Mathf.Deg2Rad * angle); // The radius of the outer circle
        float innerRadius = outerRadius / 2; // The radius of the inner circle

        // Generate directions for the outer and inner circles
        directions.AddRange(GenerateCircleDirections(baseDirection, outerRadius, outerCount));
        directions.AddRange(GenerateCircleDirections(baseDirection, innerRadius, innerCount));

        return directions;
    }

    private List<Vector3> GenerateCircleDirections(Vector3 baseDirection, float radius, int count)
    {
        List<Vector3> directions = new List<Vector3>();

        for (int i = 0; i < count; i++)
        {
            float t = i / (float)count; // The fraction around the circle (between 0 and 1)
            float angleRad = t * 2 * Mathf.PI; // The angle in radians

            Vector3 direction = new Vector3(radius * Mathf.Cos(angleRad), radius * Mathf.Sin(angleRad), 1f).normalized;
            Vector3 rotatedDirection = Quaternion.FromToRotation(Vector3.forward, baseDirection) * direction;

            directions.Add(rotatedDirection);
        }

        return directions;
    }
    
    void DrawTracer(Vector3 from, Vector3 to)
    {
        GameObject t = Instantiate(tracer, tracerSource.position, Quaternion.identity);
        t.GetComponent<LineRenderer>().SetPosition(0, from);
        t.GetComponent<LineRenderer>().SetPosition(1, to);
        Destroy(t, tracerTtl);
    }
}
