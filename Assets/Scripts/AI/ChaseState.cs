using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChaseState : StateMachineBehaviour
{
    NavMeshAgent nm;
    MobAi mobAI;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        mobAI = animator.gameObject.GetComponent<MobAi>();
        nm = mobAI.nm;
        if (mobAI.target != null)
        {
            Vector3 surroundPos = mobAI.positioningManager.GetSurroundPositionForAi(mobAI);
            nm.SetDestination(surroundPos);
        }
    }
}
