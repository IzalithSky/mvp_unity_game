using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupHeal : MonoBehaviour
{
    public int healAmount = 50;

    private void OnTriggerEnter(Collider other) {
        Damageable db = other.GetComponentInChildren<Damageable>();
        if (db != null) {
            db.Heal(healAmount);
            Destroy(gameObject);
        }
    }
}
