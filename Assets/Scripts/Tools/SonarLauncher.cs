using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonarLauncher : Tool, ReceiverInterface
{
    public GameObject projectilePrefab;
    public DamageSource damageSource;
    public float fireForce = 20f;
    
    public List<Sonar> sonars;
    public int currentSonarIndex = 0; // Add this field to keep track of the active sonar


    protected override void StartRoutine()
    {
        base.StartRoutine();
    }

    protected override void FireReady() {
        GameObject proj = Instantiate(projectilePrefab, firePoint.position, (null !=  lookPoint) ? lookPoint.rotation : firePoint.rotation);
        proj.GetComponent<PorjectileCarrier>().owners = damageSource.owners;
        proj.GetComponent<PorjectileCarrier>().damage = damageSource.DealDamage();
        proj.GetComponent<PorjectileCarrier>().damageType = damageSource.damageType;
        proj.GetComponent<PorjectileCarrier>().headMultiplier = damageSource.headMultiplier;
        proj.GetComponent<PorjectileCarrier>().receiver = this;
        proj.GetComponent<Rigidbody>().AddForce(((null !=  lookPoint) ? lookPoint.forward : firePoint.forward) * fireForce, ForceMode.Impulse);
    }

    public override void Switch() {
        CycleSonars();
    }

    public void receivePayload(GameObject payload) {
        Sonar s = payload.GetComponent<Sonar>();
        if (null != s) {
            sonars.Add(s);
        }
    }

    public void CycleSonars() 
    {
    }

    public void CleanUpDeadSonars()
    {
        sonars.RemoveAll(sonar => sonar == null);
    }
}
