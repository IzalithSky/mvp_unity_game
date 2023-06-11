using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileLauncher : Tool {
    public GameObject projectilePrefab;
    public DamageSource damageSource;

    public float fireForce = 20f;

    protected override void FireReady() {
        GameObject proj = Instantiate(projectilePrefab, firePoint.position, (null !=  lookPoint) ? lookPoint.rotation : firePoint.rotation);
        proj.GetComponent<Projectile>().owner = damageSource.owner;
        proj.GetComponent<Projectile>().damage = damageSource.DealDamage();
        proj.GetComponent<Projectile>().damageType = damageSource.damageType;
        proj.GetComponent<Projectile>().headMultiplier = damageSource.headMultiplier;
        proj.GetComponent<Rigidbody>().AddForce(((null !=  lookPoint) ? lookPoint.forward : firePoint.forward) * fireForce, ForceMode.Impulse);
    }
}
