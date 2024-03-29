using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CaptureZone : MonoBehaviour
{
    public ZoneTracker zoneTracker;
    
    public float captureTime = 5f;
    public List<string> capturingTags;

    private float currentCaptureTime = 0f;
    private bool isCapturing = false;
    private Coroutine captureCoroutine;

    public Guid spawnerID;

    public static event Action<Guid> OnCapture;

    private void Awake()
    {
        zoneTracker.zones.Add(this);
    }

    private void OnDestroy()
    {
        zoneTracker.zones.Remove(this);
    }

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
            ScannerIndication si = other.gameObject.GetComponent<ScannerIndication>();
            if (si != null)
            {
                si.isInZone = true;
            }
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
            ScannerIndication si = other.gameObject.GetComponent<ScannerIndication>();
            if (si != null)
            {
                si.isInZone = false;
            }
            if (captureCoroutine != null)
            {
                StopCoroutine(captureCoroutine);
                captureCoroutine = null;
            }
        }
    }
}
