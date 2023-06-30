using UnityEngine;

public class ProjectileDot : Projectile {
    public float tickRate = 2f;
    public int damagePerTick = 1;
    public float dotDuration = 8f;
    
    public override bool TryHit(GameObject go) {
        bool hit = base.TryHit(go);
        Damageable d = go.GetComponentInParent<Damageable>();
        if (d != null) {
            StatusController sc = go.GetComponentInParent<StatusController>();
            if (sc != null) {
                StatusDot dot = go.AddComponent<StatusDot>();
                dot.duration = dotDuration;
                dot.damageable = d;
                dot.tickRate = tickRate;
                dot.damagePerTick = damagePerTick;
                sc.ApplyStatus(dot);
            }
        }
        return hit;
    }
}