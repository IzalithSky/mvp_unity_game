using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileLauncher : Tool {
    public GameObject projectilePrefab;
    public DamageSource damageSource;
    public Rigidbody veloctiySource;

    public float fireForce = 20f;
    public bool inheritSpeed = false;

    protected override void StartRoutine() {
        base.StartRoutine();

        if (!veloctiySource) {
            veloctiySource = GetComponentInParent<Rigidbody>();
        }
    }

    protected override void FireReady() {
        GameObject proj = Instantiate(projectilePrefab, firePoint.position, (null !=  lookPoint) ? lookPoint.rotation : firePoint.rotation);
        proj.GetComponent<Projectile>().owners = damageSource.owners;
        proj.GetComponent<Projectile>().damage = damageSource.DealDamage();
        proj.GetComponent<Projectile>().damageType = damageSource.damageType;
        proj.GetComponent<Projectile>().headMultiplier = damageSource.headMultiplier;
        
        Vector3 initialForce = ((null !=  lookPoint) ? lookPoint.forward : firePoint.forward) * fireForce;
        if (inheritSpeed && veloctiySource) {
            initialForce += veloctiySource.velocity;
        }
        
        proj.GetComponent<Rigidbody>().AddForce(initialForce, ForceMode.Impulse);
    }
}
