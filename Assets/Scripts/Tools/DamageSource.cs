using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DamageType {
    Blunt,
    Piercing,
    Explosive,
    Fire
}

public class DamageSource : Tool {
    public int damage = 40;
    public int multiplier = 1;
    public int headMultiplier = 1;
    public DamageType damageType = DamageType.Blunt;

    public int DealDamage() {
        return damage * multiplier;
    }

    public int GetMultiplier() {
        return multiplier;
    }

    public void SetMultiplier(int multiplier) {
        this.multiplier = multiplier;
    }

    protected virtual void TryHit(GameObject go) {
        Damageable d = go.GetComponentInParent<Damageable>();
        if (d != null) {
            if (go.CompareTag("Head")) {
                d.Hit(DealDamage() * headMultiplier);
            } else {
                d.Hit(DealDamage());
            }
        }
    }
}
