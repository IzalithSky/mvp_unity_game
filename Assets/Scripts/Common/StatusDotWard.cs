using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusDotWard : Status
{
    public PlayerHp playerHp;
    public float tickRate;
    public int damagePerTick;

    float nextTickTime = 0f;

    StatusHotWard hot;

    public override void Apply() {
        if (hot == null) {
            hot = gameObject.GetComponent<StatusHotWard>();
        }

        if (Time.time >= nextTickTime) {            
            if (hot == null) {
                playerHp.HitWard(damagePerTick);
            }
            nextTickTime = Time.time + (1 / tickRate);
        }
    }
}
