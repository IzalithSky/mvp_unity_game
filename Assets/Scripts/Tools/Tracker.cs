using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tracker : Tool
{    
    public ProbeRenderer probeRenderer;
    public SonarRenderer sonarRenderer;

    public override void Switch() {
        sonarRenderer.autoScale = !sonarRenderer.autoScale;
    }
}
