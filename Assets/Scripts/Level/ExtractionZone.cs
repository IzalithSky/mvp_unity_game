using UnityEngine;
using System;
using System.Collections;

public class ExtractionZone : MonoBehaviour {
    public SceneLoader sceneLoader;

    public float captureTime = 5f; // The time required to capture the zone.
    private bool isPlayerInside = false;

    public static event Action OnCapture; // Event to be triggered when zone is captured

    private IEnumerator CaptureProcess() // Coroutine to handle capture process
    {
        yield return new WaitForSeconds(captureTime);
        if (isPlayerInside) {
            Capture();
        }
    }

    private void Capture()
    {
        OnCapture?.Invoke(); // Trigger the OnCapture event
        Destroy(gameObject); // Destroy the current zone after it is captured
        sceneLoader.LoadWin();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player") {
            isPlayerInside = true;
            StartCoroutine(CaptureProcess());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Player") {
            isPlayerInside = false;
            StopCoroutine(CaptureProcess());
        }
    }
}

