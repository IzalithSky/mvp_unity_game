using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtractionZone : MonoBehaviour {
    public SceneLoader sceneLoader;
    
    void OnTriggerEnter(Collider other) {

        if (other.CompareTag("Player")) {
            sceneLoader.LoadWin();
        }
    }
}

