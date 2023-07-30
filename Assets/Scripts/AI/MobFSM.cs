using System.Collections;
using UnityEngine;

public class MobFSM : MonoBehaviour 
{
    public enum State 
    {
        Idle,
        Patrol,
        Chase,
        Surround,
        Dodge,
        Flee,
        Attack,
        Staggered
    }

    public State currentState = State.Patrol;
    PathfindingModule pathfindingModule;
    PerceptionModule perceptionModule;

    public float patrolDuration = 10f;
    public float idleDuration = 5f;
    public float chaseDuration = 5f;

    private float patrolTimer = 0f;
    private float idleTimer = 0f;
    private float chaseTimer = 0f;

    public float chaseCooldownDuration = 5f;
    private float chaseCooldownTimer = 0f;

    void Start() 
    {
        pathfindingModule = GetComponent<PathfindingModule>();
        perceptionModule = GetComponent<PerceptionModule>();
        
        if (pathfindingModule == null) 
        {
            Debug.LogError("PathfindingModule is not attached to the same GameObject as MobFSM!");
            return;
        }
    }

    void Update() 
    {
        if (chaseCooldownTimer > 0f)
        {
            chaseCooldownTimer -= Time.deltaTime;
        }

        SelectState();
        DoState();
    }

    void SelectState() 
    {
        switch (currentState) 
        {
            case State.Patrol:
                patrolTimer += Time.deltaTime;

                if (patrolTimer >= patrolDuration)
                {
                    TransitionToState(State.Idle);
                    patrolTimer = 0;
                }
                else if (perceptionModule.GetClosestTarget() && chaseCooldownTimer <= 0f)
                {
                    TransitionToState(State.Chase);
                }
                break;
            
            case State.Idle:
                idleTimer += Time.deltaTime;

                if (idleTimer >= idleDuration)
                {
                    TransitionToState(State.Patrol);
                    idleTimer = 0;
                }
                else if (perceptionModule.GetClosestTarget() && chaseCooldownTimer <= 0f)
                {
                    TransitionToState(State.Chase);
                }
                break;
            
            case State.Chase:
                chaseTimer += Time.deltaTime;

                if (chaseTimer >= chaseDuration)
                {
                    TransitionToState(State.Patrol);
                    chaseCooldownTimer = chaseCooldownDuration;
                    chaseTimer = 0;
                }
                break;
        }
    }

    void DoState() 
    {
        switch (currentState) 
        {
            case State.Idle:
                ExecuteIdleState();
                break;
            case State.Patrol:
                ExecutePatrolState();
                break;
            case State.Chase:
                ExecuteChaseState();
                break;
        }
    }

    private void ExecuteIdleState() 
    {
        if (Random.Range(0f, 1f) < 0.02f) 
        {
            pathfindingModule.IdleMove();
        }
    }

    private void ExecutePatrolState() 
    {
        pathfindingModule.Patrol();
    }

    private void ExecuteChaseState() 
    {
        GameObject target = perceptionModule.GetClosestTarget();
        if (target) 
        {
            pathfindingModule.agent.SetDestination(target.transform.position);
        } 
        else 
        {
            TransitionToState(State.Patrol);
        }
    }

    public void TransitionToState(State newState) 
    {
        currentState = newState;
    }
}
