using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

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

    public Image topImage;
    public Image bottomImage;
    public Image leftImage;
    public Image rightImage;

    public Image topLeftImage;
    public Image topRightImage;
    public Image bottomLeftImage;
    public Image bottomRightImage;


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

        if (null != capturePointSpawner) {
            zoneCapturedText.text = 
                capturePointSpawner.zonesCapturedTotal.ToString() + 
                "/" + 
                capturePointSpawner.zonesCapturedMax.ToString();
        }
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

    public void ShowDamageDirection(Vector3 damageSourcePosition)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(damageSourcePosition);
        Vector3 direction = (screenPos - new Vector3(Screen.width / 2, Screen.height / 2, 0)).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Angle correction to match the UI images
        if(angle < 0) 
        {
            angle += 360;
        }

        // Reset images
        ResetImages();

        // Determine which image to show
        if (angle <= 22.5 || angle > 337.5)
            ShowImage(rightImage);
        else if (angle <= 67.5)
            ShowImage(topRightImage);
        else if (angle <= 112.5)
            ShowImage(topImage);
        else if (angle <= 157.5)
            ShowImage(topLeftImage);
        else if (angle <= 202.5)
            ShowImage(leftImage);
        else if (angle <= 247.5)
            ShowImage(bottomLeftImage);
        else if (angle <= 292.5)
            ShowImage(bottomImage);
        else if (angle <= 337.5)
            ShowImage(bottomRightImage);
    }

    private void ShowImage(Image image)
    {
        // Make the image visible
        image.color = new Color(image.color.r, image.color.g, image.color.b, 1f);
    }

    private void ResetImages()
    {
        // Make all images transparent
        Image[] images = new Image[] {topImage, bottomImage, leftImage, rightImage, topLeftImage, topRightImage, bottomLeftImage, bottomRightImage};
        foreach(Image image in images)
        {
            if(image != null) image.color = new Color(image.color.r, image.color.g, image.color.b, 0f);
        }
    }
}
