using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEffects : MonoBehaviour {
    public float amount = 4f;
    public float maxamount = 4f;
    public float smooth = 4f;
    public InputListener inputListener;
    public float damageShakeDuration = 0.2f;
    public float damageShakeMagnitude = 0.05f;
    public float explosionShakeDuration = 0.2f;
    public float explosionShakeMagnitude = 0.05f;
    public float explosionSensitivityDistance = 10f;
    
    Quaternion def;
    bool isShaking = false;


    void Start() {
        def = transform.localRotation;
    }

    void OnEnable() {
        ProjectileExplosive.OnExplosion += ExplosionShake;
    }

    void OnDisable() {
        ProjectileExplosive.OnExplosion -= ExplosionShake;
    }

    void Update() {
        float factorZ = -inputListener.GetInputHorizontal() * amount;
        factorZ = Mathf.Clamp(factorZ, -maxamount, maxamount);
        
        Quaternion final = Quaternion.Euler(0, 0, def.z + factorZ);

        transform.localRotation = Quaternion.Slerp(transform.localRotation, final, Time.deltaTime * amount * smooth);  
    }

    
    public bool IsShaking {
        get { return isShaking; }
    }
    
    public IEnumerator Shake(float duration, float magnitude) {
        if (isShaking)
            yield break;

        isShaking = true;

        Vector3 originalPos = transform.localPosition;
        float elapsed = 0.0f;

        while (elapsed < duration) {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(x, y, originalPos.z);
            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = originalPos;

        isShaking = false;
    } 

    public void ReceiveDamageShake() {
        if (!IsShaking) {
            StartCoroutine(Shake(damageShakeDuration, damageShakeMagnitude));
        }
    }

    public void ExplosionShake(Vector3 explosionPosition) {
        if (!IsShaking && Vector3.Distance(transform.position, explosionPosition) <= explosionSensitivityDistance) {
            StartCoroutine(Shake(explosionShakeDuration, explosionShakeMagnitude));
        }
    }
}
