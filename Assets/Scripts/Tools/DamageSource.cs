using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DamageType {
    Blunt,
    Piercing,
    Explosive,
    Fire
}

public class DamageSource : MonoBehaviour {
    public int damage = 40;
    public int multiplier = 1;
    public int headMultiplier = 1;
    public Collider owner;
    public DamageType damageType = DamageType.Blunt;
    public float damageDropoffStart = 10f;
    public float damageDropoffEnd = 20f;

    public int DealDamage() {
        return damage * multiplier;
    }

    public int DealDamage(float distanceFactor) {
        return Mathf.Max(0, (int)(damage * multiplier * distanceFactor));
    }

    public int GetMultiplier() {
        return multiplier;
    }

    public void SetMultiplier(int multiplier) {
        this.multiplier = multiplier;
    }

    public virtual void TryHit(GameObject go) {
        Damageable d = go.GetComponentInParent<Damageable>();
        if (d != null) {
            // Calculate the distance to the Damageable
            float distance = Vector3.Distance(transform.position, d.transform.position);
            
            // Calculate the distanceFactor based on the dropoff start and end
            float distanceFactor = 1f;
            if (distance > damageDropoffStart) {
                distanceFactor = Mathf.Clamp01(1 - ((distance - damageDropoffStart) / (damageDropoffEnd - damageDropoffStart)));
            }

            // Calculate the damage to deal
            int damageToDeal = DealDamage(distanceFactor);

            if (go.CompareTag("Head")) {
                d.Hit(damageType, damageToDeal * headMultiplier);
            } else {
                d.Hit(damageType, damageToDeal);
            }
        }
    }
}
