using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class UpdateFireworkGradient : MonoBehaviour
{
    public Transform target;
    
    VisualEffect vfx;

    void Start() {
        vfx = GetComponent<VisualEffect>();
        GameObject extractionObject = GameObject.Find("Extraction");
        if (extractionObject) {
            target = extractionObject.transform;
        }
    }

    void Update()
    {
        if (target) {
            Vector3 targetDirection = (target.position - vfx.transform.position).normalized;
            vfx.SetVector3("TargetDirection", targetDirection);
        }
    }
}
