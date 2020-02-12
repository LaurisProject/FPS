using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement_FPS : MonoBehaviour
{
    public float speed = 5f;
    public float jump_Force = 10f;
    private CharacterController character_Controller;
    private Vector3 move_Direction;
    private float gravity = 20f;
    private float vertical_Velocity;

    private void Awake()
    {
        character_Controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        MoveThePlayer();
    }

    private void MoveThePlayer()
    {
        // Get input axis.
        move_Direction = new Vector3(Input.GetAxis(Axis.HORIZONTAL), 0f, Input.GetAxis(Axis.VERTICAL));
        // Transforms direction from local space to world space.
        move_Direction = transform.TransformDirection(move_Direction);
        // Smoothing the movement and controlling the move speed.
        move_Direction *= speed * Time.deltaTime;

        ApplyGravity(); // <- Note these calls before the method ends, this is the start of a chain that can modify the value passing to the character controller.
        // Applying culculated movement to the character controller.
        character_Controller.Move(move_Direction);
    }

    private void ApplyGravity()
    {
        // If we are on the ground.
        if (character_Controller.isGrounded)
        {
            // Calculating gravity and allow to jump.
            vertical_Velocity -= gravity * Time.deltaTime;
            PlayerJump(); // <- Here is the second part of the chain.
        }
        else
        {
            // Calculating gravity.
            vertical_Velocity -= gravity * Time.deltaTime;
        }
        
        // Applying gravity.
        move_Direction.y = vertical_Velocity * Time.deltaTime;
    }

    private void PlayerJump()
    {
        // If we are on the ground and press the space key.
        if (character_Controller.isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            // Calculate jump force.
            vertical_Velocity = jump_Force;
        }
    }
}