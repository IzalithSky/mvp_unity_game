using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileDot : Projectile
{
    public float tickRate = 2f;
    public int damagePerTick = 1;
    public float dotDuration = 8f;
    
    protected override void TryHit(GameObject go)
    {
        Damageable d = go.GetComponent<Damageable>();

        if (d != null) {
            d.Hit(damage);

            StatusController sc = go.GetComponent<StatusController>();

            if (sc != null) {
                StatusDot dot = go.AddComponent<StatusDot>();
                dot.duration = dotDuration;
                dot.damageable = d;
                dot.tickRate = tickRate;
                dot.damagePerTick = damagePerTick;

                sc.ApplyStatus(dot);
            }
        }
    }
}
