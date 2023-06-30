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

    public float fadeDuration = 1f;


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
        // Get player's position and forward/right direction
        Vector3 playerPos = Camera.main.transform.position;
        Vector3 playerForward = Camera.main.transform.forward;
        Vector3 playerRight = Camera.main.transform.right;

        // Get the direction from the player to the damage source
        Vector3 direction = (damageSourcePosition - playerPos).normalized;

        // Calculate the dot products
        float forwardDot = Vector3.Dot(direction, playerForward);
        float rightDot = Vector3.Dot(direction, playerRight);

        // Determine which direction the damage came from
        if (forwardDot > 0.7f)
        {
            // Damage came from the front (top)
            LightIndicator(topImage);
        }
        else if (forwardDot < -0.7f)
        {
            // Damage came from the back (bottom)
            LightIndicator(bottomImage);
        } 
        else if (rightDot > 0.7f)
        {
            // Damage came from the right
            LightIndicator(rightImage);
        }
        else if (rightDot < -0.7f)
        {
            // Damage came from the left
            LightIndicator(leftImage);
        }
        // Check for corner directions
        else if(Mathf.Abs(forwardDot) <= 0.7f && Mathf.Abs(rightDot) <= 0.7f) {
            if(forwardDot > 0 && rightDot > 0) {
                // Front-Right (Top-Right)
                LightIndicator(topRightImage);
            } else if(forwardDot > 0 && rightDot < 0) {
                // Front-Left (Top-Left)
                LightIndicator(topLeftImage);
            } else if(forwardDot < 0 && rightDot > 0) {
                // Back-Right (Bottom-Right)
                LightIndicator(bottomRightImage);
            } else if(forwardDot < 0 && rightDot < 0) {
                // Back-Left (Bottom-Left)
                LightIndicator(bottomLeftImage);
            }
        }
    }


    void LightIndicator(Image image) {
        image.color = new Color(image.color.r, image.color.g, image.color.b, 0.5f);
        StartCoroutine(FadeImageInAndOut(image, fadeDuration));
    }

    IEnumerator FadeImageInAndOut(Image image, float duration)
    {        
        // Fade image out
        float elapsedTime = 0f;
        while(elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0.5f, 0f, elapsedTime / duration);
            image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
            yield return null;
        }
    }
}
