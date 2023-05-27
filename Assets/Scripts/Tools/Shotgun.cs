using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : DamageSource
{
    public GameObject tracer;
    public ParticleSystem muzzleFlash;
    public GameObject impactFlash;
    public GameObject bmark;
    public float bmarkTtl = 20f;
    public float tracerDistance = 100f;
    public float tracerTtl = 0.05f;
    public int pelletCount = 16; // Number of projectiles to fire in the spread
    public float spreadAngle = 8f; // Spread angle in degrees

    protected override void FireReady()
    {
        muzzleFlash.Play();

        for (int i = 0; i < pelletCount; i++)
        {
            Vector3 spreadDirection = CalculateSpreadDirection(lookPoint.forward);
            RaycastHit hit;

            if (Physics.Raycast(lookPoint.position, spreadDirection, out hit))
            {
                DrawTracer(firePoint.position, hit.point);

                GameObject impfl = Instantiate(impactFlash, hit.point + (hit.normal * .001f), Quaternion.LookRotation(hit.normal));
                Destroy(impfl, impfl.GetComponent<ParticleSystem>().main.duration);

                GameObject bm1 = Instantiate(bmark, hit.point + (hit.normal * .001f), Quaternion.FromToRotation(Vector3.up, hit.normal));
                bm1.transform.parent = hit.transform.gameObject.transform;
                Destroy(bm1, bmarkTtl);

                TryHit(hit.collider.gameObject);
            }
            else
            {
                Vector3 tracerEnd = firePoint.position + spreadDirection * tracerDistance;
                DrawTracer(firePoint.position, tracerEnd);
            }
        }
    }

    Vector3 CalculateSpreadDirection(Vector3 baseDirection)
    {
        Quaternion spreadRotation = Quaternion.Euler(Random.Range(-spreadAngle, spreadAngle), Random.Range(-spreadAngle, spreadAngle), 0f);
        return spreadRotation * baseDirection;
    }

    void DrawTracer(Vector3 from, Vector3 to)
    {
        GameObject t = Instantiate(tracer, firePoint.position, Quaternion.identity);
        t.GetComponent<LineRenderer>().SetPosition(0, from);
        t.GetComponent<LineRenderer>().SetPosition(1, to);
        Destroy(t, tracerTtl);
    }

    void TryHit(GameObject go)
    {
        Damageable d = go.GetComponent<Damageable>();

        if (d != null)
        {
            d.Hit(DealDamage());
        }
    }
}
