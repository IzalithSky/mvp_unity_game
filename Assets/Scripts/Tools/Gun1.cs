using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun1 : DamageSource {
    public GameObject tracer;
    public ParticleSystem muzzleFlash;
    public GameObject impactFlash;
    public GameObject bmark;
    public float bmarkTtl = 20f;
    public float tracerDistance = 100f;
    public float tracerTtl = 0.05f;

    protected override void FireReady() {
        muzzleFlash.Play();

        RaycastHit hit;
        if (Physics.Raycast(lookPoint.position, lookPoint.forward, out hit)) {
            DrawTracer(firePoint.position, hit.point);

            GameObject impfl = Instantiate(impactFlash, hit.point + (hit.normal * .001f), Quaternion.LookRotation(hit.normal));
            Destroy(impfl, impfl.GetComponent<ParticleSystem>().main.duration);

            GameObject bm1 = Instantiate(bmark, hit.point + (hit.normal * .001f), Quaternion.FromToRotation(Vector3.up, hit.normal));
            bm1.transform.parent = hit.transform.gameObject.transform;
            Destroy(bm1, bmarkTtl);

            TryHit(hit.collider.gameObject);
        } else {
            DrawTracer(firePoint.position, lookPoint.forward * tracerDistance);
        }
    }

    void DrawTracer(Vector3 from, Vector3 to) {
        GameObject t = Instantiate(tracer, firePoint.position, Quaternion.identity);
        t.GetComponent<LineRenderer>().SetPosition(0, from);
        t.GetComponent<LineRenderer>().SetPosition(1, to);
        Destroy(t, tracerTtl);
    }

    void TryHit(GameObject go) {
        Damageable d = go.GetComponent<Damageable>();

        if (d != null) {
            d.Hit(damageType, DealDamage());
        }
    }
}
