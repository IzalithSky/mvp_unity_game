using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusExtraDamage : Status
{
    public int multiplier = 2;
    public DamageSource damageSource;

    int originalMultiplier;

    public override void Apply()
    {
        if (multiplier != damageSource.GetMultiplier()) {
            originalMultiplier = damageSource.GetMultiplier();
            damageSource.SetMultiplier(multiplier);
        }
    }

    public override void RemoveStatus()
    {
        damageSource.SetMultiplier(originalMultiplier);
    }
}
