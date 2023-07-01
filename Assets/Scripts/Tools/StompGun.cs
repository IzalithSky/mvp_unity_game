using UnityEngine;



public class StompGun : Tool
{
    public GameObject damageZonePrefab;

    void Update() {
        if (IsReady()) {
            Fire();
        }
    }

    protected override void FireReady() {
        Instantiate(damageZonePrefab, transform.position, Quaternion.identity);
    }
}
