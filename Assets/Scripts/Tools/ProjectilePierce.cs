using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePierce : Projectile
{
    private void OnTriggerEnter(Collider c)
    {
        TryHit(c.gameObject);
    }
}
