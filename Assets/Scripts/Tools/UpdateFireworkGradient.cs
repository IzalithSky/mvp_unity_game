using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class UpdateFireworkGradient : MonoBehaviour
{
    public string targetTag = "Extraction";
    public Transform target;
    
    VisualEffect vfx;

    void Start() {
        vfx = GetComponent<VisualEffect>();

        GameObject extractionObject = FindClosestWithTag();
        if (extractionObject) {
            target = extractionObject.transform;
        }
    }

    GameObject FindClosestWithTag() {
        GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag(targetTag);

        GameObject closest = null;
        float closestDistanceSqr = Mathf.Infinity;

        foreach (GameObject obj in taggedObjects) {
            Vector3 directionToTarget = obj.transform.position - transform.position;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr) {
                closestDistanceSqr = dSqrToTarget;
                closest = obj;
            }
        }

        return closest;
    }


    void Update()
    {
        if (target) {
            Vector3 targetDirection = (target.position - vfx.transform.position).normalized;
            vfx.SetVector3("TargetDirection", targetDirection);
        }
    }
}
