using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageZone : MonoBehaviour
{
    public DamageSource damageSource;
    public float activationDelay = 1.0f;   // Time after which the damage zone becomes active
    public float damageInterval = 1.0f;    // Interval of time between each damage infliction
    public float lifetime = 2.0f;

    public bool isActive = false;
    public int len = 0;

    float to = 0f;
    HashSet<Damageable> insideObjects = new HashSet<Damageable>();

    private void Start()
    {
        to = Time.time;
        StartCoroutine(ActivationDelayCoroutine());
    }

    private IEnumerator ActivationDelayCoroutine()
    {
        yield return new WaitForSeconds(activationDelay);
        isActive = true;
        Destroy(gameObject, lifetime);
    }

    private void FixedUpdate()
    {
        len = insideObjects.Count;
        if (isActive && Time.time - to >= damageInterval)
        {
            to = Time.time;
            foreach (Damageable damageableObject in new HashSet<Damageable>(insideObjects))  // Create a copy for iteration
            {
                if (damageableObject != null)  // Check if the object is not destroyed
                {
                    damageSource.Hit(damageableObject);
                }
                else  // If the object is destroyed, remove it from the set
                {
                    insideObjects.Remove(damageableObject);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Damageable damageableObject = other.GetComponentInParent<Damageable>();
        if (damageableObject != null && !insideObjects.Contains(damageableObject))
        {
            insideObjects.Add(damageableObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Damageable damageableObject = other.GetComponentInParent<Damageable>();
        if (damageableObject != null)
        {
            insideObjects.Remove(damageableObject);
        }
    }
}
