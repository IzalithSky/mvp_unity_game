using UnityEngine;

public class PlayerSoundEffects : MonoBehaviour
{
    public MovementController mc;
    public AudioSource windSoundSource;
    public AudioSource slidingSoundSource;
    public AudioSource ambientSoundSource;
    public AudioSource stepsAudioSource;
    public bool scaleVolume = false;
    public float maxSpeed = 100f;
    public float minSpeedForWindSound = 5f;
    public float minSpeedForSlidingSound = 10f;

    private Rigidbody rb;

    private void Start()
    {
        mc = GetComponent<MovementController>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // Calculate speed as a percentage of max speed
        float speedPercentPitch = rb.velocity.magnitude / maxSpeed;
        float speedPercentVolume = scaleVolume ? (rb.velocity.magnitude / maxSpeed) : 1f;

        // Adjust wind sound based on speed
        if (rb.velocity.magnitude > minSpeedForWindSound)
        {
            if (!windSoundSource.isPlaying)
            {
                windSoundSource.Play();
            }
            windSoundSource.pitch = 1 + (speedPercentPitch / 2); // Change this value to get the desired effect
            windSoundSource.volume = speedPercentVolume;
        }
        else
        {
            windSoundSource.Stop();
        }

        // Adjust sliding sound based on speed and whether the player is grounded
        // Assume IsGrounded() is a method that checks if the player is touching the ground
        if (rb.velocity.magnitude > minSpeedForSlidingSound && mc.grounded && mc.isCrouching && mc.crouchSlidesEnabled)
        {
            if (!slidingSoundSource.isPlaying)
            {
                slidingSoundSource.Play();
            }
            slidingSoundSource.pitch = 1 + (speedPercentPitch / 2); // Change this value to get the desired effect
            slidingSoundSource.volume = speedPercentVolume;
        }
        else
        {
            slidingSoundSource.Stop();
        }

        if (!ambientSoundSource.isPlaying) {
            ambientSoundSource.Play();
        }
    }
}
