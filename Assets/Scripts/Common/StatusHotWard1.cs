using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusHotWard : Status
{
    public PlayerHp playerHp;
    public float tickRate;
    public int regenPerTick;

    float nextTickTime = 0f;

    public override void Apply()
    {
        if (Time.time >= nextTickTime)
        {            
            playerHp.HealWard(regenPerTick);
            nextTickTime = Time.time + (1 / tickRate);
        }
    }
}
