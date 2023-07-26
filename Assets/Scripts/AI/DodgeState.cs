using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DodgeState : StateMachineBehaviour
{
    NavMeshAgent nm;
    MobAi mobAI;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        mobAI = animator.gameObject.GetComponent<MobAi>();
        nm = mobAI.nm;
        mobAI.DoStrafing();
    }
}
