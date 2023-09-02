using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusDotWard : Status
{
    public PlayerHp playerHp;
    public float tickRate;
    public int damagePerTick;

    float nextTickTime = 0f;

    public override void Apply()
    {
        if (Time.time >= nextTickTime)
        {            
            playerHp.HitWard(damagePerTick);
            nextTickTime = Time.time + (1 / tickRate);
        }
    }
}
