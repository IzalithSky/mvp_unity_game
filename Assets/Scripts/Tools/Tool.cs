using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tool : MonoBehaviour {
    public float fireRateRps = 1f;
    public Transform firePoint;
    public Transform lookPoint;
    public string toolName;
    public int ammoCount = 0;
    public bool usesAmmo = false;

    protected bool ready = true;
    protected float t1;

    void Start () {
        Debug.Log("Tool here");
        t1 = Time.time; 
    }

    public void Fire() {
        if (usesAmmo && ammoCount <= 0) {
            return;
        }

        if (!ready) {
            if ((Time.time - t1) >= (1 / fireRateRps)) {
                ready = true;
            }
        }

        if (ready) {
            t1 = Time.time;
            ready = false;
            
            if (usesAmmo) {
                ammoCount--;
            }

            FireReady();
        }
    }

    public bool IsReady() {
        if (!ready) {
            if ((Time.time - t1) >= (1 / fireRateRps)) {
                ready = true;
            }
        }
        return ready;
    }

    protected virtual void FireReady() {}
}
