using UnityEngine;

public class ProjectileExplosive : Projectile {
    public GameObject explosion;

    private void OnCollisionEnter(Collision c) {
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
        foreach (Collider hit in colliders) {
            TryHit(hit.gameObject);
        }

        Destroy(gameObject);
    }
}