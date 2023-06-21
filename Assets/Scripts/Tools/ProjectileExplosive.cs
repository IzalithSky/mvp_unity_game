using UnityEngine;
using System;
using System.Collections.Generic;

public class ProjectileExplosive : Projectile {
    public GameObject explosion;
    public static event Action<Vector3> OnExplosion = delegate { };

    private void OnCollisionEnter(Collision c) {
        OnExplosion.Invoke(transform.position);

        GameObject impfl = Instantiate(impactFlash, c.contacts[0].point, Quaternion.LookRotation(c.contacts[0].normal));
        Destroy(impfl, impfl.GetComponent<ParticleSystem>().main.duration);
        
        GameObject bm1 = Instantiate(
            bmark, 
            c.contacts[0].point + (c.contacts[0].normal * .001f), 
            transform.rotation);
        bm1.transform.parent = c.transform;
        Destroy(bm1, bmarkTtl);
        
        GameObject e1 = Instantiate(explosion, c.contacts[0].point, Quaternion.LookRotation(c.contacts[0].normal));
        Destroy(e1, 1f);

        Collider[] colliders = Physics.OverlapSphere(c.contacts[0].point, splashRadius);
        
        HashSet<Damageable> uniqueDamageables = new HashSet<Damageable>();
        foreach (Collider hit in colliders) {
            Damageable d = hit.GetComponentInParent<Damageable>();
            if (d != null && !uniqueDamageables.Contains(d)) {
                uniqueDamageables.Add(d);
            }
        }

        foreach (Damageable d in uniqueDamageables) {
            TryHit(d.gameObject);
        }

        Destroy(gameObject);
    }
}