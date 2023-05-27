using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusDamageAffinity : Status
{
    public Damageable damagable;
    public DamageType damageType = DamageType.Blunt;
    public int affinity = 1;

    public override void Apply() {
        if (!damagable.damageAffinityMap.ContainsKey(damageType)) {
            damagable.damageAffinityMap.Add(damageType, affinity);
        }
    }

    public override void RemoveStatus() {
        if (damagable.damageAffinityMap.ContainsKey(damageType)) {
            damagable.damageAffinityMap.Remove(damageType);
        }
    }
}
