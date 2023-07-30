using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PerceptionModule : MonoBehaviour
{
    public List<GameObject> targetsInView;
    public float viewRadius = 10.0f;
    public float autoDetectRadius = 3.0f;
    public float viewAngle = 120.0f;
    public LayerMask targetMask;
    public float memoryDuration = 5f;

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

    public bool IsTargetInView(GameObject target)
    {
        Vector3 directionToTarget = (target.transform.position - transform.position).normalized;
        if (Vector3.Angle(transform.forward, directionToTarget) < viewAngle / 2)
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
            if (distanceToTarget <= autoDetectRadius) {
                return true;
            } 
            if (distanceToTarget <= viewRadius) {
                return Physics.Raycast(transform.position, directionToTarget, distanceToTarget, transparentMask);
            }
        }
        return false;
    }

    public void UpdateTargetsInView()
    {
        Collider[] targetsInRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

        foreach (Collider target in targetsInRadius)
        {
            if (IsTargetInView(target.gameObject))
            {
                if (!targetsInView.Contains(target.gameObject))
                {
                    targetsInView.Add(target.gameObject);
                }

                var rememberedTarget = rememberedTargets.FirstOrDefault(rt => rt.target == target.gameObject);
                if (rememberedTarget != null)
                {
                    rememberedTarget.lastSeenTime = Time.time;
                }
                else
                {
                    rememberedTargets.Add(new TargetMemory { target = target.gameObject, lastSeenTime = Time.time });
                }
            }
        }
    }

    public void PruneOldTargets()
    {
        for (int i = rememberedTargets.Count - 1; i >= 0; i--)
        {
            if (Time.time - rememberedTargets[i].lastSeenTime > memoryDuration)
            {
                targetsInView.Remove(rememberedTargets[i].target);
                rememberedTargets.RemoveAt(i);
            }
        }
    }

    public GameObject GetClosestTarget()
    {
        if (targetsInView == null || targetsInView.Count == 0)
        {
            return null;
        }

        GameObject closestTarget = targetsInView.OrderBy(t => Vector3.Distance(transform.position, t.transform.position)).First();

        return closestTarget;
    }
}
