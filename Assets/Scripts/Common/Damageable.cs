using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damageable : MonoBehaviour {
    public int maxHp = 100;
    public Dictionary<DamageType, int> damageAffinityMap = new Dictionary<DamageType, int>();
    
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
        if (damageAffinityMap.ContainsKey(damageType)) {
            damage *= damageAffinityMap[damageType];
        }

        if (damage > 0) {
            hp -= damage;
            Debug.Log(damage);
        }
        if (!IsAlive()) {
            Die();
        }
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
