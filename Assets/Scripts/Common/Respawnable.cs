using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawnable : Damageable
{
    public Transform respawn;

    public override void Die() {
        hp = maxHp;
        transform.position = respawn.position;
        transform.rotation = respawn.rotation;
    }
}
