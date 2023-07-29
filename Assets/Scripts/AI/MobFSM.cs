using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobFSM : MonoBehaviour {
    public enum State {
        Idle,
        Patrol,
        Chase,
        Surround,
        Dodge,
        Flee,
        Attack,
        Staggered
    }


    public State currentState = State.Idle;
    PathfindingModule pathfindingModule;

    public float patrolDuration = 10f;
    public float idleDuration = 5f;


    void Start() {
        pathfindingModule = GetComponent<PathfindingModule>();
        if (pathfindingModule == null) {
            Debug.LogError("PathfindingModule is not attached to the same GameObject as MobFSM!");
            return;
        }

        StartCoroutine(BehaviorLoop());
    }

    private System.Collections.IEnumerator BehaviorLoop() {
        while (true) {
            TransitionToState(State.Patrol);
            yield return new WaitForSeconds(patrolDuration);

            TransitionToState(State.Idle);
            yield return new WaitForSeconds(idleDuration);
        }
    }

    private void Update() {
        switch (currentState) {
            case State.Idle:
                ExecuteIdleState();
                break;
            case State.Patrol:
                ExecutePatrolState();
                break;
        }
    }

    private void ExecuteIdleState() {
        if (Random.Range(0f, 1f) < 0.02f) {
            pathfindingModule.IdleMove();
        }
    }

    private void ExecutePatrolState() {
        pathfindingModule.Patrol();
    }

    public void TransitionToState(State newState) {
        currentState = newState;
    }
}
