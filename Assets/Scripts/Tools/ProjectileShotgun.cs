using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileShotgun : DamageSource
{
    public GameObject projectilePrefab;
    public float fireForce = 20f;

    public int pelletCount = 8; // Number of projectiles to fire in the spread
    public float spreadAngle = 8f; // Spread angle in degrees
    public float coneRadius = 0.1f; // Radius of the cone spread

    protected override void FireReady()
    {
        for (int i = 0; i < pelletCount; i++)
        {
            Quaternion spreadRotation = Quaternion.Euler(Random.Range(-spreadAngle, spreadAngle), Random.Range(-spreadAngle, spreadAngle), 0f);
            Vector3 spreadDirection = spreadRotation * ((null != lookPoint) ? lookPoint.forward : firePoint.forward);

            Vector3 randomOffset = Random.insideUnitCircle * coneRadius;
            Vector3 spawnPosition = firePoint.position + randomOffset;

            GameObject proj = Instantiate(projectilePrefab, spawnPosition, (null != lookPoint) ? lookPoint.rotation : firePoint.rotation);
            proj.GetComponent<Projectile>().launcher = owner;
            proj.GetComponent<Projectile>().damage = DealDamage();
            proj.GetComponent<Projectile>().damageType = damageType;
            proj.GetComponent<Rigidbody>().AddForce(spreadDirection * fireForce, ForceMode.Impulse);
        }
    }
}
