using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyable : Damageable
{
    public int staggerDamageThreshold = 100;
    public float staggerDuration = 1f;
    public bool isStaggered = false;
    
    int staggerDamage = 0;

    protected override void TakeDamageRaw(int damage) {
        base.TakeDamageRaw(damage);
        
        staggerDamage += damage;
        if (staggerDamage >= staggerDamageThreshold) {
            isStaggered = true;
            StartCoroutine(ResetStaggerAfterSeconds(staggerDuration));
        }
    }


    IEnumerator ResetStaggerAfterSeconds(float seconds) {
        yield return new WaitForSeconds(seconds);
        isStaggered = false;
        staggerDamage = 0;
    }

    public override void Die() {
        Destroy(gameObject);
    }
}
