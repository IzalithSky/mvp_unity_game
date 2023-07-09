using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioPlayer : MonoBehaviour
{
    public AudioClip[] audioClips;
    public InputListener inputListener;  // Add a reference to the InputListener

    public AudioSource audioSource;
    private int currentClipIndex = 0;


    private void Update() {
        if (inputListener.GetIsPlayNext()) {
            PlayClipNext();
        }

        if (inputListener.GetIsPlayStop()) {
            Stop();
        }
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
