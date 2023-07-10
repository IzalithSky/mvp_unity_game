using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioPlayer : MonoBehaviour
{
    public AudioClip[] audioClips;
    public AudioSource audioSource;
    
    int currentClipIndex = 0;
    PlayerControls playerControls;


    void Awake()
    {
        playerControls = new PlayerControls();

        playerControls.Menus.PlayNext.performed += ctx => PlayClipNext();
        playerControls.Menus.PlayStop.performed += ctx => Stop();

        playerControls.Enable();
    }

    void OnDestroy()
    {
        playerControls.Dispose();
    }

    public void PlayClipNext() {
        PlayClip(currentClipIndex);
        currentClipIndex = (currentClipIndex + 1) % audioClips.Length;
    }

    public void PlayClip(int clipIndex)
    {
        if (clipIndex < 0 || clipIndex >= audioClips.Length)
        {
            Debug.LogWarning("Clip index out of range.");
            return;
        }

        audioSource.clip = audioClips[clipIndex];
        audioSource.loop = true;  // Enable looping
        audioSource.Play();
    }

    public void Stop() {
        audioSource.loop = false;  // Disable looping
        audioSource.Stop();
    }
}
