using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicsSettings : MonoBehaviour
{
    public bool limitFps = false;
    public bool vSync = false;
    public int fpsCap = 0;


    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        
        if (limitFps) {
            Application.targetFrameRate = fpsCap;
        }

        if (vSync) {
            QualitySettings.vSyncCount = 1;
        }
    }
}
