using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CaptureZone : MonoBehaviour
{
    public float captureTime = 5f;
    public List<string> capturingTags;

    private float currentCaptureTime = 0f;
    private bool isCapturing = false;
    private Coroutine captureCoroutine;

    public Guid spawnerID; // unique ID of the spawner that created this zone

    public static event Action<Guid> OnCapture;

    public float CapturePercentage
    {
        get { return currentCaptureTime / captureTime; }
    }

    private IEnumerator CaptureProcess()
    {
        while (currentCaptureTime < captureTime)
        {
            yield return new WaitForEndOfFrame();
            if (isCapturing)
            {
                currentCaptureTime += Time.deltaTime;
            }
        }
        OnCapture?.Invoke(spawnerID);
        Destroy(gameObject);
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
