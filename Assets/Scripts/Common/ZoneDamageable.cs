using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneDamageable : MonoBehaviour {
    // This Dictionary will hold the Damageable objects for different zones
    public Dictionary<string, Damageable> damageZones = new Dictionary<string, Damageable>();

    // This method allows you to add new zones with specific damage multipliers
    public void AddDamageZone(string zoneName, int zoneMaxHP, Dictionary<DamageType, int> zoneDamageAffinityMap) {
        Damageable zone = new Damageable();
        zone.maxHp = zoneMaxHP;
        zone.damageAffinityMap = zoneDamageAffinityMap;
        damageZones.Add(zoneName, zone);
    }

    // This method allows you to apply damage to a specific zone
    public void HitZone(string zoneName, DamageType damageType, int damage) {
        if(damageZones.ContainsKey(zoneName)){
            damageZones[zoneName].Hit(damageType, damage);
        } else {
            Debug.LogError("Zone not found: " + zoneName);
        }
    }
}
