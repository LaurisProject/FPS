using UnityEngine;

public class PlayerFootsteps_FPS : MonoBehaviour
{
    private AudioSource footstep_Sound;
    [SerializeField] private AudioClip[] footstep_Clip;

    private CharacterController character_Controller;

    [HideInInspector] public float volume_Min, volume_Max;

    private float accumulated_Distance;

    [HideInInspector] public float step_Distance;

    private void Awake()
    {
        footstep_Sound = GetComponent<AudioSource>();
        character_Controller = GetComponentInParent<CharacterController>();
    }

    private void Start()
    {

    }

    private void Update()
    {
        CheckToPlayFootstepSound();
    }

    private void CheckToPlayFootstepSound()
    {
        // If we're not on the ground, return.
        if (!character_Controller.isGrounded)
            return;

        // If we are moving, no matter in which direction.
        if (character_Controller.velocity.sqrMagnitude > 0)
        {
            // Accumulated distance is how far you can move before triggering the footstep.
            // In other words, the time your leg takes to move before your foot hits the ground.
            // For example, running will accumulate less distance because your legs are moving faster.
            accumulated_Distance += Time.deltaTime;
            
            // If that distance is over the predetermined step distance play a footstep sound.
            if (accumulated_Distance > step_Distance)
            {
                footstep_Sound.volume = Random.Range(volume_Min, volume_Max);
                footstep_Sound.clip = footstep_Clip[Random.Range(0, footstep_Clip.Length)];
                footstep_Sound.Play();

                accumulated_Distance = 0;
            }
        }
        else
        {
            accumulated_Distance = 0f;
        }
    }
}