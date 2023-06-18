using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {
    public Collider player;
    public List<GameObject> mobExamples;
    public Transform spawnPoint;
    public float respawnCooldown = 2;
    public bool automaticSpawning = false;
    public bool respawnOnEnter = false;

    float respwanTime = 0;

    void Start() {
        respwanTime = Time.time;
    }

    private void Update() {
        if (automaticSpawning) {
            if (Time.time - respwanTime >= respawnCooldown) {
                SpwanMobs();
                respwanTime = Time.time;
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (respawnOnEnter) {
            if (Time.time - respwanTime >= respawnCooldown) {
                SpwanMobs();
                respwanTime = Time.time;
            }
        }
    }

    void SpwanMobs() {
        foreach (GameObject ex in mobExamples) {
            GameObject mob = Instantiate(ex, spawnPoint.position, spawnPoint.rotation);
            mob.GetComponent<MobAi>().player = player;
        }
    }
}
