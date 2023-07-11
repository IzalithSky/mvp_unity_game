using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

public class ProjectileExplosive : Projectile {
    public GameObject explosion;
    public float pushForce = 10f;
    public float fuseDuration = 0f;  // Time in seconds before the projectile explodes
    public static event Action<Vector3> OnExplosion = delegate { };


    protected override void StartRoutine() {
        base.StartRoutine();

        // Start the fuse coroutine only if fuseDuration is greater than 0
        if (fuseDuration > 0f) {
            StartCoroutine(Fuse());
        }
    }

    void OnCollisionEnter(Collision c) {
        // Stop the fuse coroutine if it is running
        if (fuseDuration > 0f) {
            StopCoroutine(Fuse());
        }
        ExplodeAtPoint(c.contacts[0].point, c.contacts[0].normal, c.transform);
        Destroy(gameObject);
    }

    IEnumerator Fuse() {
        yield return new WaitForSeconds(fuseDuration);
        ExplodeAtPoint(transform.position, transform.forward);  // Use transform.forward as the normal
        Destroy(gameObject);
    }

    public void ExplodeAtPoint(Vector3 point, Vector3 normal, Transform parent = null) {
        OnExplosion.Invoke(point);

        GameObject impfl = Instantiate(impactFlash, point, Quaternion.LookRotation(normal));
        Destroy(impfl, impfl.GetComponent<ParticleSystem>().main.duration);
        
        GameObject bm1 = bmarkAlignsWithProjectile ?
            Instantiate(bmark, point + (normal * .001f), transform.rotation) :
            Instantiate(bmark, point + (normal * .001f), Quaternion.identity);
        if (bmarkFollows && parent != null) {
            bm1.transform.parent = parent;
        }
        Destroy(bm1, bmarkTtl);
        
        GameObject e1 = Instantiate(explosion, point, Quaternion.LookRotation(normal));
        Destroy(e1, 1f);

        Collider[] colliders = Physics.OverlapSphere(point, splashRadius);
        
        HashSet<Damageable> uniqueDamageables = new HashSet<Damageable>();
        HashSet<Rigidbody> uniqueRbs = new HashSet<Rigidbody>();
        foreach (Collider hit in colliders) {
            Damageable d = hit.GetComponentInParent<Damageable>();
            if (d != null && !uniqueDamageables.Contains(d)) {
                uniqueDamageables.Add(d);
            }
            Rigidbody rb = hit.GetComponentInParent<Rigidbody>();
            if (rb != null && !uniqueRbs.Contains(rb)) {
                uniqueRbs.Add(rb);
            }
        }

        foreach (Damageable d in uniqueDamageables) {
            TryHit(d.gameObject);
        }

        foreach (Rigidbody rb in uniqueRbs) {
            rb.AddForce((rb.position - point).normalized * pushForce, ForceMode.Impulse);
        }
    }
}
