using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolState : StateMachineBehaviour
{
    NavMeshAgent nm;
    MobAi mobAI; // Reference to the main MobAi component

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        mobAI = animator.gameObject.GetComponent<MobAi>();
        nm = mobAI.nm;
        Vector3 rndPos = mobAI.transform.position + Random.insideUnitSphere * mobAI.walkRadius;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(rndPos, out hit, mobAI.pathFndRadius, NavMesh.AllAreas))
        {
            nm.SetDestination(hit.position);
        }
    }
}
