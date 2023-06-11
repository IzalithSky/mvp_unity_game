using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : DamageSource {
    public float timeoutSec = 4;
    public GameObject impactFlash;
    public GameObject bmark;
    public float bmarkTtl = 20f;
    public float splashRadius = 5f;

    public Collider ignored1;
    public Collider ignored2;


    void Start() {
        int projLayer = LayerMask.NameToLayer("Projectiles");
        Physics.IgnoreLayerCollision(projLayer, projLayer);
        
        Physics.IgnoreCollision(GetComponent<Collider>(), owner);
        ignored1 = GetComponent<Collider>();
        ignored2 = owner;
        
        Destroy(gameObject, timeoutSec);
    }

    private void OnCollisionEnter(Collision c) {
        GameObject impfl = Instantiate(impactFlash, c.contacts[0].point, Quaternion.LookRotation(c.contacts[0].normal));
        Destroy(impfl, impfl.GetComponent<ParticleSystem>().main.duration);
        
        GameObject bm1 = Instantiate(
            bmark, 
            c.contacts[0].point + (c.contacts[0].normal * .001f), 
            transform.rotation);
        bm1.transform.parent = c.transform;
        Destroy(bm1, bmarkTtl);
        
        TryHit(c.gameObject);
        
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, splashRadius);
    }
}