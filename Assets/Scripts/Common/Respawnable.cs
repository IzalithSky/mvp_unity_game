using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawnable : Damageable
{
    public Transform respawn;
    public SceneLoader sceneLoader;

    public override void Die() {
        if (null == sceneLoader) {
            hp = maxHp;
            transform.position = respawn.position;
            transform.rotation = respawn.rotation;
        } else {
            sceneLoader.LoadLose();
        }
    }
}
