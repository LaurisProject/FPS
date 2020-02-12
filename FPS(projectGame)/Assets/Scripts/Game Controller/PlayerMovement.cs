using Photon.Pun;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private PhotonView PV;
    public float speed = 5f;
    public float jumpForce = 10f;
    private CharacterController _characterController;
    private Vector3 _moveDirection;
    private float _gravity = 20f;
    private float _verticalVelocity;

    private void Start()
    {
        PV = GetComponent<PhotonView>();
        _characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (PV.IsMine)
        {
            Movement();
        }
    }
    private void Movement()
    {
        _moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        _moveDirection = transform.TransformDirection(_moveDirection);
        _moveDirection *= speed * Time.deltaTime;

        ApplyGravity();

        _characterController.Move(_moveDirection);
    }

    private void ApplyGravity()
    {
        if (_characterController.isGrounded)
        {
            _verticalVelocity -= _gravity * Time.deltaTime;
            PlayerJump();
        }
        else
        {
            _verticalVelocity -= _gravity * Time.deltaTime;
        }

        _moveDirection.y = _verticalVelocity * Time.deltaTime;
    }

    private void PlayerJump()
    {
        if (_characterController.isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            _verticalVelocity = jumpForce;
        }
    }
}
