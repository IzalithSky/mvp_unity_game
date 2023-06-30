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
    float lastSeenTime = 0f;
    float lastHitTime = 0f;
    bool preFireDelayPassed = false;

    protected override void StartRoutine()
    {
        base.StartRoutine();

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
            if (hit.collider.gameObject == target.gameObject) {
                return true;
            }
        }
        
        return false;
    }

    protected override void FireReady() {
        hadLineOfSight = hasLineOfSight;
        hasLineOfSight = HasLineOfSight();
        
        // If target has just entered line of sight, start the countdown
        if (hasLineOfSight && !hadLineOfSight) {
            lastSeenTime = Time.time;
        }

        // If target has left line of sight, reset countdown
        if (!hasLineOfSight && hadLineOfSight) {
            lastSeenTime = 0;
        }

        // If target has stayed in line of sight for preFireDelaySeconds, deal damage
        if (hasLineOfSight && Time.time - lastSeenTime >= preFireDelaySeconds) {
            damageSource.TryHit(target.gameObject);
        }
    }
}
