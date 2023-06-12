using UnityEngine;
using System;
using System.Collections;

public class DamageZone : MonoBehaviour
{
    public float activeTime = 10f;
    public float damageInterval = 0.2f;
    public int damageAmount = 2;
    private bool isPlayerInside = false;
    private Damageable playerDamageable = null;

    public static event Action OnDespawn;

    private IEnumerator DamageProcess() {
        while (isPlayerInside) {
            if (playerDamageable != null) {
                playerDamageable.Hit(damageAmount);
            }
            yield return new WaitForSeconds(damageInterval);
        }
    }

    private void Start() {
        StartCoroutine(DespawnProcess());
    }

    private IEnumerator DespawnProcess() {
        yield return new WaitForSeconds(activeTime);
        Despawn();
    }

    private void Despawn() {
        OnDespawn?.Invoke(); // Trigger the OnDespawn event
        Destroy(gameObject); // Destroy the current zone after it is captured
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player") {
            isPlayerInside = true;
            playerDamageable = other.GetComponent<Damageable>();
            StartCoroutine(DamageProcess());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Player") {
            isPlayerInside = false;
            StopCoroutine(DamageProcess());
        }
    }
}
