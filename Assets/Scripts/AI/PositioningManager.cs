using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "PositioningManager", menuName = "ScriptableObjects/PositioningManager")]
public class PositioningManager : ScriptableObject
{
    public float surroundRadius = 3f;
    public int updateFrequency = 10;

    private Dictionary<Collider, List<MobAi>> targetGroups = new Dictionary<Collider, List<MobAi>>();
    private int frameCounter = 0;

    // Register AI with the manager
    public void RegisterAi(MobAi ai)
    {
        if (ai.target != null)
        {
            if (!targetGroups.ContainsKey(ai.target))
                targetGroups[ai.target] = new List<MobAi>();
            
            targetGroups[ai.target].Add(ai);
        }
    }

    public void UnregisterAi(MobAi ai)
    {
        if (ai.target != null && targetGroups.ContainsKey(ai.target))
        {
            targetGroups[ai.target].Remove(ai);
        }
    }

    // Called by AI to get its surrounding position
    public Vector3 GetSurroundPositionForAi(MobAi ai)
    {
        if (ai.target == null) return ai.transform.position;

        // Only calculate new positions every nth frame
        frameCounter = (frameCounter + 1) % updateFrequency;
        if (frameCounter != 0) return ai.nm.destination;

        if (targetGroups.ContainsKey(ai.target))
        {
            List<MobAi> group = targetGroups[ai.target];
            int index = group.IndexOf(ai);

            if (index != -1)
            {
                float angleStep = 360f / group.Count;
                float angle = index * angleStep * Mathf.Deg2Rad;

                Vector3 dir = new Vector3(Mathf.Sin(angle), 0f, Mathf.Cos(angle));
                return ai.target.transform.position + dir * surroundRadius;
            }
        }
        return ai.transform.position;
    }
}
