using UnityEngine;

public class ProjectileDot : Projectile {
    public float tickRate = 2f;
    public int damagePerTick = 1;
    public float dotDuration = 8f;
    
    protected override void TryHit(GameObject go) {
        base.TryHit(go);
        Damageable d = go.GetComponent<Damageable>();
        if (d != null) {
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