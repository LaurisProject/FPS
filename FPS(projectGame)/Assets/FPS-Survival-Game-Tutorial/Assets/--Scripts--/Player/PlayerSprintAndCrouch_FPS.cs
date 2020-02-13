using System;
using UnityEngine;

public class PlayerSprintAndCrouch_FPS : MonoBehaviour
{
    private PlayerMovement player_Movement;

    public float sprint_Speed = 10f;
    public float move_Speed = 5f;
    public float crouch_Speed = 2f;

    private Transform look_Root;
    private float stand_Height = 1.6f;
    private float crouch_Height = 1f;
    private bool is_Crouching;

    private void Awake()
    {
        player_Movement = GetComponent<PlayerMovement>();
        look_Root = transform.GetChild(0);
    }

    private void Update()
    {
        Sprint();
        Crouch();
    }

    private void Sprint()
    {
        // Only allow the character to sprint if not crouching.
        if (Input.GetKeyDown(KeyCode.LeftShift) && !is_Crouching)
        {
            player_Movement.speed = sprint_Speed;
        }
        // If the sprint key is released, stop sprinting.
        if (Input.GetKeyUp(KeyCode.LeftShift) && !is_Crouching)
        {
            player_Movement.speed = move_Speed;
        }
    }

    private void Crouch()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            // If the character is crouched, stand up.
            if (is_Crouching)
            {
                look_Root.localPosition = new Vector3(0f, stand_Height, 0f); // This is only changing the camera position.
                player_Movement.speed = move_Speed;
                is_Crouching = false;
            }
            // Else crouch.
            else
            {
                look_Root.localPosition = new Vector3(0f, crouch_Height, 0f);
                player_Movement.speed = crouch_Speed;
                is_Crouching = true;
            }
        }
    }
}