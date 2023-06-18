using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {
    public Collider player;
    public List<GameObject> mobExamples;
    public Transform spawnPoint;
    public float respawnCooldown = 2;

    // New variable to control automatic spawning
    public bool automaticSpawning = false;

    float respwanTime = 0;

    void Start() {
        respwanTime = Time.time;
        if (automaticSpawning) {
            StartCoroutine(SpawnMobsAutomatically());
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (Time.time - respwanTime >= respawnCooldown) {
            SpwanMobs();
            respwanTime = Time.time;
        }
    }

    void SpwanMobs() {
        foreach (GameObject ex in mobExamples) {
            GameObject mob = Instantiate(ex, spawnPoint.position, spawnPoint.rotation);
            mob.GetComponent<MobAi>().player = player;
            // mob.GetComponent<EnemyAI>().player = player.transform;
        }
    }

    // New coroutine for automatic spawning
    IEnumerator SpawnMobsAutomatically() {
        while (true) {
            yield return new WaitForSeconds(respawnCooldown);
            SpwanMobs();
        }
    }
}
