using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PerceptionModule : MonoBehaviour
{
    public Transform lookPoint;
    public float losSearchProjectileSize = 0.01f;
    public float viewRadius = 10.0f;
    public float autoDetectRadius = 3.0f;
    public float viewAngle = 120.0f;
    public LayerMask targetMask;
    public List<string> targetTags = new List<string> { "Player" };
    public float memoryDuration = 5f;
    
    List<GameObject> targetsInView = new List<GameObject>();

    class TargetMemory
    {
        public GameObject target;
        public float lastSeenTime;
    }

    LayerMask transparentMask;
    List<TargetMemory> rememberedTargets = new List<TargetMemory>();


    void Start() {
        string[] transparentLayers = new string[] { "Tools", "Projectiles", "Trigger" };
        transparentMask = ~LayerMask.GetMask(transparentLayers); 
    }

    void Update()
    {
        UpdateTargetsInView();
        PruneOldTargets();
    }

    public GameObject GetClosestTarget()
    {
        if (targetsInView.Count == 0)
        {
            return null;
        }

        GameObject closestTarget = targetsInView.OrderBy(t => Vector3.Distance(transform.position, t.transform.position)).First();

        return closestTarget;
    }

    bool IsTargetInView(GameObject target)
    {
        Vector3 directionToTarget = (target.transform.position - transform.position).normalized;
        float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
        if (distanceToTarget <= autoDetectRadius) {
            return true;
        }
        if (distanceToTarget <= viewRadius) {
            if (Vector3.Angle(transform.forward, directionToTarget) < viewAngle / 2) {
                return HasLineOfSight();
            }
        }
        return false;
    }

    bool HasLineOfSight() {
        if (!GetClosestTarget()) {
            return false;
        }
        return HasLineOfSight(lookPoint.position, GetClosestTarget().transform.position);
    }

    bool HasLineOfSight(Vector3 fromPosition, Vector3 toPosition) {
        Vector3 direction = toPosition - fromPosition;
        RaycastHit hit;
        if (Physics.SphereCast(fromPosition, losSearchProjectileSize, direction, out hit, Mathf.Infinity, transparentMask)) {
            Debug.DrawRay(fromPosition, hit.point - fromPosition, Color.cyan);
            return true;
        }
        
        return false;
    }

    void UpdateTargetsInView()
    {
        Collider[] targetsInRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

        foreach (Collider target in targetsInRadius)
        {
            if (targetTags.Contains(target.tag) && IsTargetInView(target.gameObject)) {
                if (!targetsInView.Contains(target.gameObject)) {
                    targetsInView.Add(target.gameObject);
                }

                TargetMemory rememberedTarget = rememberedTargets.FirstOrDefault(rt => rt.target == target.gameObject);
                if (rememberedTarget != null) {
                    rememberedTarget.lastSeenTime = Time.time;
                } else {
                    rememberedTargets.Add(new TargetMemory { target = target.gameObject, lastSeenTime = Time.time });
                }
            }
        }
    }

    void PruneOldTargets()
    {
        for (int i = rememberedTargets.Count - 1; i >= 0; i--) {
            if (Time.time - rememberedTargets[i].lastSeenTime > memoryDuration) {
                targetsInView.Remove(rememberedTargets[i].target);
                rememberedTargets.RemoveAt(i);
            }
        }
    }
}
