using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UiController : MonoBehaviour
{
    public TMP_Text hpText;
    public TMP_Text wardText;
    public PlayerHp playerHp;

    public TMP_Text toolNameText;
    public TMP_Text ammoCountText;
    public ToolHolder toolHolder;

    public TMP_Text fpsText;
    public TMP_Text velocityText;
    public Rigidbody rb;

    public TMP_Text zoneCapturedEText;
    public TMP_Text zoneCapturedPText;
    public TMP_Text zoneCapturedBText;
    public CapturePointSpawner capturePointSpawnerE;
    public CapturePointSpawner capturePointSpawnerP;

    public TMP_Text timerText; // Add this line
    public GameTimer gameTimer; // Add this line

    public Image[] directionIndicators; // 0: Top, 1: Bottom, 2: Left, 3: Right, 4: TopLeft, 5: TopRight, 6: BottomLeft, 7: BottomRight

    public float fadeDuration = 1f;

    float dt = 0.0f;
    float fps = 0.0f;
    float updateRate = 4.0f;  // 4 updates per sec.
    int frameCount = 0;

    // Track each coroutine
    private Dictionary<Image, Coroutine> imageFadingCoroutines = new Dictionary<Image, Coroutine>();


    void Start()
    {
        if (!capturePointSpawnerP && !capturePointSpawnerE) {
            capturePointSpawnerP = FindObjectOfType<CapturePointSpawner>();
        }

        if (!gameTimer) {
            gameTimer = FindObjectOfType<GameTimer>();
        }

        UpdateText(hpText, "0");
        UpdateText(wardText, "0");
        UpdateText(toolNameText, "");
        UpdateText(ammoCountText, "");
        UpdateText(fpsText, "0");
        UpdateText(velocityText, "0");
        UpdateText(zoneCapturedEText, "");
        UpdateText(zoneCapturedPText, "");
        UpdateText(zoneCapturedBText, "");
        UpdateText(timerText, "");
    }

    void Update()
    {
        UpdateText(hpText, playerHp.GetHp().ToString());
        UpdateText(wardText, playerHp.GetWard().ToString());

        if (null != toolHolder.currentTool)
        {
            UpdateText(toolNameText, toolHolder.currentTool.toolName);
            UpdateText(ammoCountText, toolHolder.currentTool.usesAmmo ? toolHolder.currentTool.ammoCount.ToString() : "∞");
        }

        UpdateFps();
        UpdateText(fpsText, fps.ToString("F2"));
        if (null != rb) {
            UpdateText(velocityText, (3.6 * rb.velocity.magnitude).ToString("F0"));
        }

        if (null != capturePointSpawnerE)
        {
            UpdateText(zoneCapturedEText, $"{capturePointSpawnerE.zonesCapturedTotal}/{capturePointSpawnerE.zonesCapturedMax}");
        }
        if (null != capturePointSpawnerP)
        {
            UpdateText(zoneCapturedPText, $"{capturePointSpawnerP.zonesCapturedTotal}/{capturePointSpawnerP.zonesCapturedMax}");
        }

        if (null != gameTimer) // If the gameTimer reference is set
        {
            // Update the timerText with the time remaining in minutes:seconds format
            UpdateText(timerText, $"{Mathf.FloorToInt(gameTimer.timeLeft / 60)}:{Mathf.FloorToInt(gameTimer.timeLeft % 60).ToString("00")}");
        }
    }

    void UpdateText(TMP_Text textComponent, string value)
    {
        textComponent.text = value;
    }

    void UpdateFps()
    {
        frameCount++;
        dt += Time.deltaTime;
        if (dt > 1.0f / updateRate)
        {
            fps = frameCount / dt;
            frameCount = 0;
            dt -= 1.0f / updateRate;
        }
    }

    public void ShowDamageDirection(Vector3 damageSourcePosition)
    {
        Vector3 direction = (damageSourcePosition - Camera.main.transform.position).normalized;
        float forwardDot = Vector3.Dot(direction, Camera.main.transform.forward);
        float rightDot = Vector3.Dot(direction, Camera.main.transform.right);

        Dictionary<string, Image> directions = new Dictionary<string, Image> {
            {"Top", directionIndicators[0]}, {"Bottom", directionIndicators[1]}, {"Left", directionIndicators[2]}, {"Right", directionIndicators[3]},
            {"TopLeft", directionIndicators[4]}, {"TopRight", directionIndicators[5]}, {"BottomLeft", directionIndicators[6]}, {"BottomRight", directionIndicators[7]}
        };

        if (forwardDot > 0.6f) LightIndicator(directions["Top"]);
        else if (forwardDot < -0.6f) LightIndicator(directions["Bottom"]);
        else if (rightDot > 0.6f) LightIndicator(directions["Right"]);
        else if (rightDot < -0.6f) LightIndicator(directions["Left"]);
        else if (Mathf.Abs(forwardDot) <= 0.6f && Mathf.Abs(rightDot) <= 0.6f) {
            if (forwardDot > 0 && rightDot > 0) LightIndicator(directions["TopRight"]);
            else if (forwardDot > 0 && rightDot < 0) LightIndicator(directions["TopLeft"]);
            else if (forwardDot < 0 && rightDot > 0) LightIndicator(directions["BottomRight"]);
            else if (forwardDot < 0 && rightDot < 0) LightIndicator(directions["BottomLeft"]);
        }
    }

    void LightIndicator(Image image)
    {
        image.color = new Color(image.color.r, image.color.g, image.color.b, 0.5f);
        // Stop the previous coroutine for the same image if it is running
        if(imageFadingCoroutines.ContainsKey(image) && imageFadingCoroutines[image] != null)
        {
            StopCoroutine(imageFadingCoroutines[image]);
            imageFadingCoroutines[image] = null;
        }
        // Start a new coroutine
        imageFadingCoroutines[image] = StartCoroutine(FadeImageInAndOut(image, fadeDuration));
    }

    IEnumerator FadeImageInAndOut(Image image, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0.5f, 0f, elapsedTime / duration);
            image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
            yield return null;
        }
        // Cleanup
        imageFadingCoroutines[image] = null;
    }
}
