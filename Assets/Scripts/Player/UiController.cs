using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UiController : MonoBehaviour
{
    public TMP_Text hpText;
    public Damageable damagable;
    
    public TMP_Text toolNameText;
    public TMP_Text ammoCountText;
    public ToolHolder toolHolder;
    
    public TMP_Text fpsText;
    public TMP_Text velocityText;
    public Rigidbody rb;

    public TMP_Text zoneCapturedText;
    public CapturePointSpawner capturePointSpawner;

    float dt = 0.0f;
    float fps = 0.0f;
    float updateRate = 4.0f;  // 4 updates per sec.
    int frameCount = 0;


    void Start()
    {
        hpText.text = "0";
        
        toolNameText.text = "";
        ammoCountText.text = "";

        fpsText.text = "0";
        velocityText.text = "0";

        zoneCapturedText.text = "";
    }

    void Update()
    {
        hpText.text = damagable.GetHp().ToString();

        if (null != toolHolder.currentTool) {
            toolNameText.text = toolHolder.currentTool.toolName;
            if (toolHolder.currentTool.usesAmmo) {
                ammoCountText.text = toolHolder.currentTool.ammoCount.ToString();
            } else {
                ammoCountText.text = "∞";
            }
        }
        
        UpdateFps();
        fpsText.text = fps.ToString("F2");
        
        velocityText.text = new Vector3(rb.velocity.x, 0f, rb.velocity.z).magnitude.ToString("F2");

        zoneCapturedText.text = 
            capturePointSpawner.zonesCapturedTotal.ToString() + 
            "/" + 
            capturePointSpawner.zonesCapturedMax.ToString();
    }

    void UpdateFps() {
        frameCount++;
        dt += Time.deltaTime;
        if (dt > 1.0f / updateRate) {
            fps = frameCount / dt;
            frameCount = 0;
            dt -= 1.0f / updateRate;
        }
    }
}
