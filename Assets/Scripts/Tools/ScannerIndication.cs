using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScannerIndication : MonoBehaviour
{
    public List<GameObject> indicators;
    public bool isInZone = false;

    void Update() {
        foreach (GameObject i in indicators) {
            i.SetActive(isInZone);
        }
    }
}
