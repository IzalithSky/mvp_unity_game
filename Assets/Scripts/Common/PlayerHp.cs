using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHp : Damageable
{
    public Transform respawn;
    public SceneLoader sceneLoader;

    public int maxWard = 100;
    public int ward;

    protected override void StartRoutine() {
        ward = maxWard;
    }

    public override void Die() {
        if (null == sceneLoader) {
            hp = maxHp;
            transform.position = respawn.position;
            transform.rotation = respawn.rotation;
        } else {
            sceneLoader.LoadLose();
        }
    }

    public int GetWard() {
        return ward;
    }

    public bool HasWard() {
        return ward > 0;
    }

    public void HitWard(int damage) {
        if (damage <= 0) {
            return;
        }

        if (ward > 0) {
            ward -= damage;
        } else {
            TakeDamageRaw(damage);
        }

        ward = ward < 0 ? 0 : ward;
    }

    protected virtual void TakeWardDamageRaw(int damage) {
        ward -= damage;
    }

    public void HealWard(int amount) {
        if (amount > 0) {
            ward += amount;
            if (ward > maxWard) {
                ward = maxWard;
            }
        }
    }
}
