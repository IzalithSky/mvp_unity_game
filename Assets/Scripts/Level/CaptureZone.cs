using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CaptureZone : MonoBehaviour
{
    public float captureTime = 5f; // The time required to capture the zone.
    public List<string> capturingTags; // List of tags that can capture the zone.

    private float currentCaptureTime = 0f;
    private bool isCapturing = false;
    private Coroutine captureCoroutine;

    public static event Action OnCapture; // Event to be triggered when zone is captured

    public float CapturePercentage
    {
        get { return currentCaptureTime / captureTime; }
    }

    public float cp = 0f;


    void Update() {
        cp = CapturePercentage;
    }


    private IEnumerator CaptureProcess() // Coroutine to handle capture process
    {
        while (currentCaptureTime < captureTime)
        {
            yield return new WaitForEndOfFrame();
            if (isCapturing)
            {
                currentCaptureTime += Time.deltaTime;
            }
        }
        OnCapture?.Invoke(); // Trigger the OnCapture event
        Destroy(gameObject); // Destroy the current zone after it is captured
    }

    private void OnTriggerEnter(Collider other)
    {
        if (capturingTags.Contains(other.gameObject.tag))
        {
            isCapturing = true;
            if (captureCoroutine == null)
            {
                captureCoroutine = StartCoroutine(CaptureProcess());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (capturingTags.Contains(other.gameObject.tag))
        {
            isCapturing = false;
            if (captureCoroutine != null)
            {
                StopCoroutine(captureCoroutine);
                captureCoroutine = null;
            }
        }
    }
}
