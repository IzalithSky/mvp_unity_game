using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Railgun : Hitscan
{
    protected override void FireReady() {
        muzzleFlash.Play();
        DrawTracer(tracerSource.position, lookPoint.forward * tracerDistance);

        RaycastHit[] hits = Physics.SphereCastAll(lookPoint.position, beamRadius, lookPoint.forward, Mathf.Infinity, mask);

        foreach (RaycastHit hit in hits) {
            GameObject impfl = Instantiate(impactFlash, hit.point + (hit.normal * .001f), Quaternion.LookRotation(hit.normal));
            Destroy(impfl, impfl.GetComponent<ParticleSystem>().main.duration);

            GameObject bm1 = Instantiate(bmark, hit.point + (hit.normal * .001f), Quaternion.FromToRotation(Vector3.up, hit.normal));
            bm1.transform.parent = hit.transform.gameObject.transform;
            Destroy(bm1, bmarkTtl);

            damageSource.TryHit(hit.collider.gameObject);
        }
    }
}
