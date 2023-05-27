using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunAnimation : MonoBehaviour {
    public Animator anim;
    public string fireAnimationName;

    public void Fire() {
        anim.Play(fireAnimationName, 0, 0);
    }
}
