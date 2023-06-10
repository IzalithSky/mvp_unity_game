using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : DamageSource {
    public GameObject impactFlash;
    public GameObject bmark;
    public GunAnimation anim;
    
    public float bmarkTtl = 20f;
    public float range = 1;
    public float splash = .2f;

    protected override void FireReady() {
        anim.Fire();

        RaycastHit hit;
        if (Physics.SphereCast(firePoint.position, splash, firePoint.forward, out hit, range)) {
            GameObject impfl = Instantiate(impactFlash, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impfl, impfl.GetComponent<ParticleSystem>().main.duration);

            GameObject bm1 = Instantiate(bmark, hit.point + (hit.normal * .001f), Quaternion.FromToRotation(Vector3.up, hit.normal));
            bm1.transform.parent = hit.transform.gameObject.transform;
            Destroy(bm1, bmarkTtl);

            TryHit(hit.collider.gameObject);
        }   
    }
}
