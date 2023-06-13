using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform player;
    public Transform toolHolder;
    public float detectionRange = 10f;
    public float attackRange = 2f;
    public float dodgeRange = 5f;
    public bool canFlee = true;
    public Tool weapon;
    public Damageable damageable;

    private NavMeshAgent navMeshAgent;
    private Vector3 randomTarget;
    private enum State { Searching, Chasing, ObtainingLineOfSight, Attacking, Dodging, Fleeing }
    private State state;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        state = State.Searching;
    }

    void Update()
    {
        switch (state)
        {
            case State.Searching:
                randomTarget = new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
                navMeshAgent.SetDestination(randomTarget);
                if (PlayerInSight() && PlayerInDetectionRange()) { state = State.Chasing; }
                break;

            case State.Chasing:
                navMeshAgent.SetDestination(player.position);
                if (!PlayerInDetectionRange()) { state = State.Searching; }
                else if (PlayerInAttackRange()) { state = State.Attacking; }
                break;

            case State.Attacking:
                FacePlayer();
                if (weapon.IsReady()) { weapon.Fire(); state = State.Dodging; }
                break;

            case State.Dodging:
                randomTarget = transform.position + new Vector3(Random.Range(-dodgeRange, dodgeRange), 0, Random.Range(-dodgeRange, dodgeRange));
                navMeshAgent.SetDestination(randomTarget);
                if (navMeshAgent.remainingDistance <= 0.2f && PlayerInAttackRange()) { state = State.Attacking; }
                else if (!PlayerInAttackRange()) { state = State.Chasing; }
                break;

            case State.Fleeing:
                randomTarget = transform.position - (player.position - transform.position).normalized * dodgeRange;
                navMeshAgent.SetDestination(randomTarget);
                if (!PlayerInDetectionRange()) { state = State.Searching; }
                break;
        }

        if (canFlee && damageable.GetHp() <= damageable.maxHp / 4 && state != State.Fleeing) { state = State.Fleeing; }
    }

    bool PlayerInDetectionRange()
    {
        return Vector3.Distance(player.position, transform.position) <= detectionRange;
    }

    bool PlayerInAttackRange()
    {
        return Vector3.Distance(player.position, transform.position) <= attackRange;
    }

    bool PlayerInSight()
    {
        RaycastHit hit;
        Vector3 direction = player.position - transform.position;
        if (Physics.Raycast(transform.position, direction, out hit, detectionRange))
        {
            return hit.transform == player;
        }
        return false;
    }

    void FacePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

        toolHolder.LookAt(player.transform);
    }
}
