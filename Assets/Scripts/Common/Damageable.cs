using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damageable : MonoBehaviour {
    public int maxHp = 100;
    public Dictionary<DamageType, int> damageAffinityMap = new Dictionary<DamageType, int>();
    public CameraEffects cameraEffects;
    public int damageShakeThreshold = 1;
    
    protected int hp = 0;


    void Start() {
        hp = maxHp;
    }

    public int GetHp() {
        return hp;
    }

    public bool IsAlive() {
        return hp >= 0;
    }

    public void Hit(int damage) {
        Hit(DamageType.Blunt, damage);
    }

    public void Hit(DamageType damageType, int damage) {
        if (null != cameraEffects && damage > damageShakeThreshold) {
            cameraEffects.ReceiveDamageShake();
        }

        if (damageAffinityMap.ContainsKey(damageType)) {
            int affinity = damageAffinityMap[damageType];
            
            if (affinity > 0) {
                damage *= affinity;
            } else if (affinity < 0) {
                damage /= Mathf.Abs(affinity);
            }
        }

        if (damage > 0) {
            TakeDamageRaw(damage);
        }
        if (!IsAlive()) {
            Die();
        }
    }

    protected virtual void TakeDamageRaw(int damage) {
        hp -= damage;
    }

    public void Heal(int amount) {
        if (amount > 0) {
            hp += amount;
            if (hp > maxHp) {
                hp = maxHp;
            }
        }
    }

    public virtual void Die() {}
}
