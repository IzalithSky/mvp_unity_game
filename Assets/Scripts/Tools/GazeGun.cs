using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GazeGun : Tool
{
    public Transform target;
    public DamageSource damageSource;
    public float preFireDelaySeconds = 2f;

    bool hasLineOfSight = false;
    bool hadLineOfSight = false;
    float firstSeenTime = 0f;
    bool canShoot = false;

    protected override void StartRoutine()
    {
        base.StartRoutine();

        string[] transparentLayers = new string[] { "Tools", "Projectiles", "Trigger" };
        mask = ~LayerMask.GetMask(transparentLayers); 

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length > 0) {
            target = players[0].GetComponentInChildren<Collider>().transform;
        }
    }

    bool HasLineOfSight() {
        return HasLineOfSight(firePoint.position, target.position);
    }

    bool HasLineOfSight(Vector3 fromPosition, Vector3 toPosition) {
        Vector3 direction = toPosition - fromPosition;  
        RaycastHit hit;        
        if (Physics.Raycast(fromPosition, direction, out hit, Mathf.Infinity, mask)) {
            Debug.DrawRay(fromPosition, hit.point - fromPosition, Color.yellow);
            if (hit.collider.gameObject == target.gameObject) {
                return true;
            }
        }
        
        return false;
    }

    void Update() {
        canShoot = false;
        hadLineOfSight = hasLineOfSight;
        hasLineOfSight = HasLineOfSight();
        
        // If target has just entered line of sight, start the countdown
        if (hasLineOfSight && !hadLineOfSight) {
            firstSeenTime = Time.time;
        }

        // If target has stayed in line of sight for preFireDelaySeconds, deal damage
        if (hasLineOfSight && Time.time - firstSeenTime >= preFireDelaySeconds) {
            canShoot = true;
        }        
    }

    protected override void FireReady() {
        if (canShoot) {
            damageSource.TryHit(target.gameObject);
        }
    }
}
