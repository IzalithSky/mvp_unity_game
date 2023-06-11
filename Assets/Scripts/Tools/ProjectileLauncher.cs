using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileLauncher : DamageSource {
    public GameObject projectilePrefab;
    public float fireForce = 20f;

    protected override void FireReady() {
        GameObject proj = Instantiate(projectilePrefab, firePoint.position, (null !=  lookPoint) ? lookPoint.rotation : firePoint.rotation);
        proj.GetComponent<Projectile>().owner = owner;
        proj.GetComponent<Projectile>().damage = DealDamage();
        proj.GetComponent<Projectile>().damageType = damageType;
        proj.GetComponent<Projectile>().headMultiplier = headMultiplier;
        proj.GetComponent<Rigidbody>().AddForce(((null !=  lookPoint) ? lookPoint.forward : firePoint.forward) * fireForce, ForceMode.Impulse);
    }
}
