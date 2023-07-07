using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PorjectileCarrier : Projectile
{
    public ReceiverInterface receiver;

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
        
        receiver.receivePayload(bm1);

        Destroy(bm1, bmarkTtl);
        
        TryHit(c.gameObject);
        
        Destroy(gameObject);
    }
}
