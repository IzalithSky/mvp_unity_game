using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class Tracker : Tool
{    
    public ProbeRenderer probeRenderer;
    public SonarRenderer sonarRenderer;

    public Canvas cameraCanvas;
    public Camera cameraFront;
    public Camera cameraTop;

    public TMP_Text outText;

    bool topView = false;


    void Awake() {
        SetActiveCameraFront();
    }

    void Update() {
        List<CaptureZone> zs = probeRenderer.GetZones();

        List<string> hexInstanceIDs = zs.Select(zone => {
            string hexID = "0x" + zone.gameObject.GetInstanceID().ToString("X");
            
            if (probeRenderer.CurrentTarget == zone) {
                hexID += " * "; // Add an asterisk to the current target
                hexID += $"({probeRenderer.DistanceToCurrentTarget}m)"; // Add the distance to the current target
            }
            return hexID;
        }).ToList();

        string output = string.Join("\n", hexInstanceIDs);
        outText.text = output;
    }

    void SwitchView() {
        if (topView) {
            SetActiveCameraFront();
        } else {
            SetActiveCameraTop();
        }

        topView = !topView;
    }

    void SetActiveCameraFront() {
        cameraFront.gameObject.SetActive(true);
        cameraTop.gameObject.SetActive(false);
        
        cameraCanvas.worldCamera = cameraFront;

        sonarRenderer.markersState.targetCamera = cameraFront.transform;

        sonarRenderer.markersState.autoScale = true;
    }

    void SetActiveCameraTop() {
        cameraTop.gameObject.SetActive(true);
        cameraFront.gameObject.SetActive(false);
        
        cameraCanvas.worldCamera = cameraTop;

        sonarRenderer.markersState.targetCamera = cameraTop.transform;

        sonarRenderer.markersState.autoScale = false;
    }

    public override void Switch() {
        probeRenderer.SetNextTarget();
    }

    public override void Mode0() {
        sonarRenderer.markersState.autoScale = !sonarRenderer.markersState.autoScale;
    }

    public override void Mode1() {
        SwitchView();
    }

    public override void Mode2() {}
}
