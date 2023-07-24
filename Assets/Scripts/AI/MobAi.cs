using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;


public enum AiBehMode {
    IDLE,
    ATTACKING,
    CHASING,
    FLEEING,
    DODGING,
    STAGGER
}


public class MobAi : MonoBehaviour {
    public NavMeshAgent nm;
    public float targetChangeInterval = 5f; // The time interval to change the target in seconds.
    public List<string> targetTags = new List<string>{"Player"}; // List of tags that this mob can target.
    public Transform toolHolder;
    public Tool tool;
    public Transform firePoint;
    public float walkRadius = 5.0f;
    public float losSearchProjectileSize = 0.5f;
    public float pathFndRadius = 1.0f;
    public float strafeDelay = 5.0f;
    public float fireingRange = 15.0f;
    public float minLosSearchRange = 0f;
    public float preAttackDelay = 0.3f;
    public Destroyable destroyable;
    public AudioClip attackSound; // Sound to play when the mob attacks
    public AudioClip deathSound; // Sound to play when the mob dies
    public AudioClip voiceSound; // Sound to play at fixed intervals
    public float voiceInterval = 10.0f; // The interval at which the voice sound is played, in seconds

    public List<Collider> targets = new List<Collider>(); // List of potential targets.
    public Collider target; // The current target.
    public AiBehMode state = AiBehMode.CHASING;
    
    public PositioningManager positioningManager;

    bool isStrafeReady = false;
    float strafeStartTime = 0.0f;

    bool isAttackReady = false;
    float attackModeStartTime = 0.0f;

    LayerMask losSearchMask;
    
    AudioSource audioSource;
    float lastVoiceTime;

    void Start () {
        strafeStartTime = Time.time;
        
        string[] transparentLayers = new string[] { "Tools", "Projectiles", "Trigger" };
        losSearchMask = ~LayerMask.GetMask(transparentLayers); 

        RefreshTargets();
        if (targets.Count > 0) 
        {
            target = targets[Random.Range(0, targets.Count)];
        }
        StartCoroutine(ChangeTargetCoroutine());

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        lastVoiceTime = Time.time;

        // If tool is not assigned, find it in the current GameObject or its children
        if (tool == null) {
            tool = GetComponentInChildren<Tool>();
        }

        if (tool == null) {
            Debug.LogWarning("No Tool found in the GameObject or its children.");
        }

        positioningManager.RegisterAi(this);
    }

    private void OnDestroy() {
        positioningManager.UnregisterAi(this);
    }

    void Update() { 
        EnsureTargetPresence();
        
        DoState();
        UpdateState();

        // Play the voice sound at fixed intervals
        if (voiceSound != null && Time.time - lastVoiceTime >= voiceInterval) {
            audioSource.PlayOneShot(voiceSound);
            lastVoiceTime = Time.time;
        }
    }

    void EnsureTargetPresence() {
        if (null == target) {
            RefreshTargets();
        }
    }

    private IEnumerator ChangeTargetCoroutine() 
    {
        while (true) 
        {
            yield return new WaitForSeconds(targetChangeInterval);
            RefreshTargets();
            if (targets.Count > 0) 
            {
                target = targets[Random.Range(0, targets.Count)];
                if (target == null)
                {
                    RefreshTargets();
                }
            }
        }
    }

    private void RefreshTargets() 
    {
        targets.Clear();
        foreach (string tag in targetTags) 
        {
            var gameObjects = GameObject.FindGameObjectsWithTag(tag);
            foreach (var go in gameObjects)
            {
                var collider = go.GetComponentInChildren<Collider>();
                if (collider != null)
                {
                    targets.Add(collider);
                }
            }
        }
    }

    void UpdateState() {
        switch (state)
        {
            case AiBehMode.CHASING:
                if (!TargetOutOfRange()) {	
                    state = AiBehMode.ATTACKING;
                    attackModeStartTime = Time.time;
                }
                break;
            case AiBehMode.ATTACKING:
                if (TargetOutOfRange() && IsPreAttackDelayOver()) {	
                    state = AiBehMode.CHASING;
                }
                if (!tool.IsReady()) {
                    state = AiBehMode.DODGING;
                }
                if (IsPreAttackDelayOver()) {
                    isAttackReady = true;
                }
                break;
            case AiBehMode.DODGING:
                if (tool.IsReady()) {
                    state = AiBehMode.ATTACKING;
                    attackModeStartTime = Time.time;
                }
                break;
            case AiBehMode.STAGGER:
                if (null != destroyable && !destroyable.isStaggered) {
                    state = AiBehMode.CHASING;
                }
                break;
            default:
                break;
        }

        if (null != destroyable && destroyable.isStaggered) {
            state = AiBehMode.STAGGER;
        }

        // Play the death sound when the mob dies
        if (null != destroyable && !destroyable.IsAlive() && deathSound != null) {
            audioSource.PlayOneShot(deathSound);
        }
    }

    bool IsPreAttackDelayOver() {
        return Time.time - attackModeStartTime >= preAttackDelay;
    }

    bool TargetOutOfRange() {
        if (target == null) {
            return true; // or whatever makes sense in your game logic
        }
        return Vector3.Distance(target.transform.position, transform.position) > (HasLineOfSight() ? fireingRange : minLosSearchRange);
    }

    void DoState() {
        EnsureTargetPresence();
        switch (state)
        {
            case AiBehMode.CHASING:
                if (target != null)  {
                    FaceTarget(target.transform);

                    Vector3 surroundPos = positioningManager.GetSurroundPositionForAi(this);
                    nm.SetDestination(surroundPos);
                }
                break;
            case AiBehMode.ATTACKING:
                if (target != null) {
                    FaceTarget(target.transform);
                    nm.SetDestination(transform.position);
                    if (isAttackReady && HasLineOfSight()) {
                        isAttackReady = false;
                        attackModeStartTime = Time.time;

                        tool.Fire();

                        // Play the attack sound when the mob attacks
                        if (attackSound != null) {
                            audioSource.PlayOneShot(attackSound);
                        }
                    }
                    EnsureTargetPresence();
                }
                break;
            case AiBehMode.DODGING:
                if (target != null) {
                    FaceTarget(target.transform);
                    DoStrafing();
                }
                break;
            case AiBehMode.STAGGER:
                nm.SetDestination(transform.position);
                break;
            default:
                break;
        }
    }

    void DoStrafing() {
        if (!isStrafeReady) {
            if (Time.time - strafeStartTime >= strafeDelay) {
                isStrafeReady = true;
            }
        }

        if (isStrafeReady) {
            strafeStartTime = Time.time;
            isStrafeReady = false;

            Vector3 rndPos = transform.position 
				+ Random.insideUnitSphere * walkRadius;
				
            NavMeshHit hit;
            if (NavMesh.SamplePosition(
					rndPos, 
					out hit, 
					pathFndRadius, 
					NavMesh.AllAreas)) {
						
                nm.SetDestination(hit.position);
            }
        }
    }

    void FaceTarget(Transform t) {
        Vector3 lookPos = t.position - transform.position;
        lookPos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(
								transform.rotation, 
								rotation, 
								nm.angularSpeed);

        toolHolder.LookAt(t);
        firePoint.LookAt(t);
    }

    bool HasLineOfSight() {
        if (target == null) {
            return false; // or whatever makes sense in your game logic
        }
        return HasLineOfSight(firePoint.position, target.bounds.center);
    }

    bool HasLineOfSight(Vector3 fromPosition, Vector3 toPosition) {
        Vector3 direction = toPosition - fromPosition;  
        RaycastHit hit;        
        if (Physics.SphereCast(fromPosition, losSearchProjectileSize, direction, out hit, Mathf.Infinity, losSearchMask)) {
            Debug.DrawRay(fromPosition, hit.point - fromPosition, Color.cyan);
            if (hit.collider.CompareTag("Player")) {
                return true;
            }
        }
        
        return false;
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(nm.destination, 0.1f);
    }
}
