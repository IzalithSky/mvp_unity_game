using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tracker : Tool
{    
    public ProbeRenderer probeRenderer;
    public SonarRenderer sonarRenderer;

    public Canvas cameraCanvas;
    public Camera cameraFront;
    public Camera cameraTop;

    bool topView = false;


    void Awake() {
        SetActiveCameraFront();
    }

    void SwitchView() {
        if (topView) {
            SetActiveCameraFront();
            sonarRenderer.markersState.autoScale = true;
        } else {
            SetActiveCameraTop();
            sonarRenderer.markersState.autoScale = false;
        }

        topView = !topView;
    }

    void SetActiveCameraFront() {
        cameraFront.gameObject.SetActive(true);
        cameraTop.gameObject.SetActive(false);
        
        cameraCanvas.worldCamera = cameraFront;

        sonarRenderer.markersState.targetCamera = cameraFront.transform;
    }

    void SetActiveCameraTop() {
        cameraTop.gameObject.SetActive(true);
        cameraFront.gameObject.SetActive(false);
        
        cameraCanvas.worldCamera = cameraTop;

        sonarRenderer.markersState.targetCamera = cameraTop.transform;
    }

    public override void Switch() {
        sonarRenderer.markersState.autoScale = !sonarRenderer.markersState.autoScale;

        SwitchView();
    }
}
