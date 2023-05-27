using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Status : MonoBehaviour 
{
    public float duration = 1f;
    public bool isPermanent = false;

    float startTime = 0f;
    bool isActive = true;

    void Start() {
        startTime = Time.time;
    }

    void FixedUpdate() {
        if (isPermanent || Time.time - startTime < duration) {
            Apply();
        } else {
            isActive = false;
        }
    }

    public bool IsActive() {
        if (isPermanent) {
            return true;
        } else {
            return isActive;
        }
    }

    public virtual void Apply() {}
    public virtual void RemoveStatus() {}
}
