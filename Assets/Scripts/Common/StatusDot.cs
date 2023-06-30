using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusDot : Status
{
    public Damageable damageable;
    public float tickRate;
    public int damagePerTick;
    public DamageType damageType = DamageType.Blunt;

    float nextTickTime = 0f;

    public override void Apply()
    {
        if (Time.time >= nextTickTime)
        {            
            damageable.Hit(damageType, damagePerTick, transform.position);
            nextTickTime = Time.time + (1 / tickRate);
        }
    }
}
