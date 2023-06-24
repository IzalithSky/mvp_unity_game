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
    public CameraEffects cameraEffects;
    public float recoilDuration = 0f;
    public float recoilMagnitude = 0f;
    public float maxRecoilAngle = 0f;

    protected bool ready = true;
    protected float t1;
    protected LayerMask mask;

    void Start () {
        t1 = Time.time;
        string[] transparentLayers = new string[] { "Tools", "Projectiles", "Trigger", "Smoke" };
        mask = ~LayerMask.GetMask(transparentLayers); 
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

            if (null != cameraEffects && recoilDuration > 0f && recoilMagnitude > 0f && maxRecoilAngle > 0f) {
                cameraEffects.PlayRecoil(recoilDuration, recoilMagnitude, maxRecoilAngle);
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
