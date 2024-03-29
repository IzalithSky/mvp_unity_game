using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : DamageSource {
    public float timeoutSec = 4;
    public GameObject impactFlash;
    public GameObject bmark;
    public float bmarkTtl = 20f;
    public float splashRadius = 5f;
    public bool bmarkFollows = true;
    public bool bmarkAlignsWithProjectile = true;


    void Start() {
        StartRoutine();
    }

    protected virtual void StartRoutine() {
        int projLayer = LayerMask.NameToLayer("Projectiles");
        Physics.IgnoreLayerCollision(projLayer, projLayer);
        
        foreach (Collider owner in owners) {
            Physics.IgnoreCollision(GetComponent<Collider>(), owner);
        }
        
        Destroy(gameObject, timeoutSec);
    }

    private void OnCollisionEnter(Collision c) {
        GameObject impfl = Instantiate(impactFlash, c.contacts[0].point, Quaternion.LookRotation(c.contacts[0].normal));
        Destroy(impfl, impfl.GetComponent<ParticleSystem>().main.duration);
            
        GameObject bm1 = bmarkAlignsWithProjectile ?
            Instantiate(
                bmark, 
                c.contacts[0].point + (c.contacts[0].normal * .001f), 
                transform.rotation) :
            Instantiate(
                bmark, 
                c.contacts[0].point + (c.contacts[0].normal * .001f), 
                Quaternion.identity);
        if (bmarkFollows) {
            bm1.transform.parent = c.transform;
        }
        
        if (bmarkTtl > 0) {
            Destroy(bm1, bmarkTtl);
        }
        
        TryHit(c.gameObject);
        
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, splashRadius);
    }
}