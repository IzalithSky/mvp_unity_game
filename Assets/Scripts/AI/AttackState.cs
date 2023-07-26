using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AttackState : StateMachineBehaviour
{
    NavMeshAgent nm;
    MobAi mobAI;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        mobAI = animator.gameObject.GetComponent<MobAi>();
        nm = mobAI.nm;
        mobAI.tool.Fire();
        // Assuming we'll also play the sound here
        if (mobAI.attackSound != null)
        {
            mobAI.audioSource.PlayOneShot(mobAI.attackSound);
        }
    }
}
