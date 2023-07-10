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

    public Camera cam;
    public float zoomSpeed = 30f;
    public float minFOV = 30f;
    public float maxFOV = 150f;
    
    Quaternion def;
    bool isShaking = false;

    PlayerControls playerControls;


    void Awake()
    {
        playerControls = new PlayerControls();

        playerControls.Menus.FoVIn.performed += ctx => ChangeFov(zoomSpeed);
        playerControls.Menus.FoVOut.performed += ctx => ChangeFov(-zoomSpeed);

        playerControls.Enable();
    }

    void OnDestroy()
    {
        playerControls.Dispose();
    }

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

    void ChangeFov(float step) {
        cam.fieldOfView += step;
        cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, minFOV, maxFOV);
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

    public IEnumerator Recoil(float recoilDuration, float recoilMagnitude, float maxRecoilAngle) {
        float elapsed = 0.0f;
        float recoilAngle = Mathf.Min(recoilMagnitude, maxRecoilAngle);
        
        while (elapsed < recoilDuration) {
            float x = Random.Range(-recoilAngle, recoilAngle);
            float y = Random.Range(-recoilAngle, recoilAngle);

            cam.transform.localRotation = Quaternion.Euler(x, y, 0f);
            elapsed += Time.deltaTime;

            yield return null;
        }

        // Return camera to its original rotation
        while (Quaternion.Angle(cam.transform.localRotation, Quaternion.identity) > 0.01f) {
            cam.transform.localRotation = Quaternion.Slerp(cam.transform.localRotation, Quaternion.identity, Time.deltaTime * smooth);
            yield return null;
        }
    }

    public void PlayRecoil(float recoilDuration, float recoilMagnitude, float maxRecoilAngle) {
        StartCoroutine(Recoil(recoilDuration, recoilMagnitude, maxRecoilAngle));
    }
}
