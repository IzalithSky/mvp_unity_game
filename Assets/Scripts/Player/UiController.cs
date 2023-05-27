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

    void Start()
    {
        hpText.text = "0";
        
        toolNameText.text = "";
        ammoCountText.text = "";
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
    }
}
