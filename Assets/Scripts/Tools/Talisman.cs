using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Talisman : Tool {
    public GunAnimation[] anims;
    public float tickRate = 2f;
    public int regenPerTick = 16;

    PlayerHp playerHp;

    protected override void FireReady() {
        if (anims.Length > 0) {
            int randomIndex = Random.Range(0, anims.Length);
            anims[randomIndex].Fire();
        }
    }

    void AtemptAttach() {
        if (playerHp == null) {
            PlayerHp p = GetComponentInParent<PlayerHp>();
            if (p != null) {
                playerHp = p;
            }            
        }
    }

    void OnEnable() {
        AtemptAttach();
        
        if (playerHp != null) {
            StatusHotWard h = playerHp.gameObject.GetComponent<StatusHotWard>();
            if (h == null) {
                StatusHotWard hot = playerHp.gameObject.AddComponent<StatusHotWard>();
                hot.isPermanent = true;
                hot.duration = 1;
                hot.playerHp = playerHp;
                hot.tickRate = tickRate;
                hot.regenPerTick = regenPerTick;
            }
        }
    }

    void OnDisable() {
        AtemptAttach();

        if (playerHp != null) {
            StatusHotWard hot = playerHp.gameObject.GetComponent<StatusHotWard>();
            if (hot != null) {
                hot.RemoveStatus();
                Destroy(hot);
            }
        }
    }
}
