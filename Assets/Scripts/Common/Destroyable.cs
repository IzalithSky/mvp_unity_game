using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyable : Damageable
{
    public override void Die() {
        Destroy(gameObject);
    }
}
