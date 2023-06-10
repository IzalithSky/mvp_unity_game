using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {
    public float timeoutSec = 4;
    public GameObject impactFlash;
    public GameObject bmark;
    public float bmarkTtl = 20f;
    public int damage = 60;
    public GameObject launcher;
    public float splashRadius = 5f;

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

    void Start() {
        int projLayer = LayerMask.NameToLayer("Projectiles");
        Physics.IgnoreLayerCollision(projLayer, projLayer);
        if (null != launcher) {
            Physics.IgnoreCollision(GetComponent<Collider>(), launcher.GetComponent<Collider>());
        }
        Destroy(gameObject, timeoutSec);
    }

    protected virtual void TryHit(GameObject go) {
        Damageable d = go.GetComponentInParent<Damageable>();
        if (d != null) {
            d.Hit(damage);
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, splashRadius);
    }
}