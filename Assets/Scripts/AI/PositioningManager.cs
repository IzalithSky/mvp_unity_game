using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "PositioningManager", menuName = "ScriptableObjects/PositioningManager")]
public class PositioningManager : ScriptableObject
{
    public float surroundRadius = 3f;
    public int updateFrequency = 10;

    private Dictionary<GameObject, List<MobAI>> targetGroups = new Dictionary<GameObject, List<MobAI>>();
    private int frameCounter = 0;

    // Register AI with the manager
    public void RegisterAi(MobAI ai)
    {
        if (ai.GetTarget() != null)
        {
            if (!targetGroups.ContainsKey(ai.GetTarget()))
                targetGroups[ai.GetTarget()] = new List<MobAI>();
            
            targetGroups[ai.GetTarget()].Add(ai);
        }
    }

    public void UnregisterAi(MobAI ai)
    {
        if (ai.GetTarget() != null && targetGroups.ContainsKey(ai.GetTarget()))
        {
            targetGroups[ai.GetTarget()].Remove(ai);
        }
    }

    // Called by AI to get its surrounding position
    public Vector3 GetSurroundPositionForAi(MobAI ai)
    {
        if (ai.GetTarget() == null) return ai.transform.position;

        // Only calculate new positions every nth frame
        frameCounter = (frameCounter + 1) % updateFrequency;
        if (frameCounter != 0) return ai.GetAgent().destination;

        if (targetGroups.ContainsKey(ai.GetTarget()))
        {
            List<MobAI> group = targetGroups[ai.GetTarget()];
            int index = group.IndexOf(ai);

            if (index != -1)
            {
                float angleStep = 360f / group.Count;
                float angle = index * angleStep * Mathf.Deg2Rad;

                Vector3 dir = new Vector3(Mathf.Sin(angle), 0f, Mathf.Cos(angle));
                return ai.GetTarget().transform.position + dir * surroundRadius;
            }
        }
        return ai.transform.position;
    }
}
