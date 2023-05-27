using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTilt : MonoBehaviour {
    public float amount = 4f;
    public float maxamount = 4f;
    public float smooth = 4f;
    public InputListener inputListener;
    
    Quaternion def;

    void Start() {
        def = transform.localRotation;
    }

    void Update() {
        float factorZ = -inputListener.GetInputHorizontal() * amount;
        factorZ = Mathf.Clamp(factorZ, -maxamount, maxamount);
        
        Quaternion final = Quaternion.Euler(0, 0, def.z + factorZ);

        transform.localRotation = Quaternion.Slerp(transform.localRotation, final, Time.deltaTime * amount * smooth);  
    } 
}
