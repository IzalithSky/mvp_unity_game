using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitscan : Tool {
    public GameObject tracer;
    public ParticleSystem muzzleFlash;
    public GameObject impactFlash;
    public GameObject bmark;
    public Transform tracerSource;
    public float bmarkTtl = 20f;
    public float tracerDistance = 100f;
    public float tracerTtl = 0.05f;
    public DamageSource damageSource;
    public float beamRadius = 0.01f;

    protected override void FireReady() {
        muzzleFlash.Play();

        RaycastHit hit;
        if (Physics.SphereCast(lookPoint.position, beamRadius, lookPoint.forward, out hit, Mathf.Infinity, mask)) {
            DrawTracer(tracerSource.position, hit.point);

            GameObject impfl = Instantiate(impactFlash, hit.point + (hit.normal * .001f), Quaternion.LookRotation(hit.normal));
            Destroy(impfl, impfl.GetComponent<ParticleSystem>().main.duration);

            GameObject bm1 = Instantiate(bmark, hit.point + (hit.normal * .001f), Quaternion.FromToRotation(Vector3.up, hit.normal));
            bm1.transform.parent = hit.transform.gameObject.transform;
            Destroy(bm1, bmarkTtl);

            damageSource.TryHit(hit.collider.gameObject);
        } else {
            DrawTracer(tracerSource.position, lookPoint.position + lookPoint.forward * tracerDistance);
        }
    }

    protected void DrawTracer(Vector3 from, Vector3 to) {
        GameObject t = Instantiate(tracer, tracerSource.position, Quaternion.identity);
        t.GetComponent<LineRenderer>().SetPosition(0, from);
        t.GetComponent<LineRenderer>().SetPosition(1, to);
        Destroy(t, tracerTtl);
    }
}
