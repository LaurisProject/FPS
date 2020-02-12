using Photon.Pun;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [SerializeField] private PhotonView PV;
    [SerializeField] private Transform _playerRoot;
    [SerializeField] private Transform _lookRoot;
    [SerializeField] private bool _invert;
    [SerializeField] private bool _canUnlock = true;
    [SerializeField] private float _sensivity = 5f;
    [SerializeField] private int _smoothStep = 10;
    [SerializeField] private float _smoothWeight = 0.4f;
    [SerializeField] private float _rollAngle = 10f;
    [SerializeField] private float _rollSpeed = 3f;
    [SerializeField] private Vector2 _defaultLookLimits = new Vector2(-70f, 80f);

    private Vector2 _lookAngles;
    private Vector2 _currentMouseLook;
    private Vector2 _smoothMove;
    private float _currentRollAngle;
    private int _lasLookFrame;

    private void Awake()
    {
        PV = GetComponentInParent<PhotonView>();
    }

    private void Start()
    {
        if (PV.IsMine)
            Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (PV.IsMine)
        {
            LockAndUnlockCursor();
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                LookAround();
            }
        }
    }

    private void LockAndUnlockCursor()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }

    private void LookAround()
    {
        _currentMouseLook = new Vector2(Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"));

        _lookAngles.x += _currentMouseLook.x * _sensivity * (_invert ? 1f : -1f);
        _lookAngles.y += _currentMouseLook.y * _sensivity;

        // Limits how far we can roll on the X axis.
        _lookAngles.x = Mathf.Clamp(_lookAngles.x, _defaultLookLimits.x, _defaultLookLimits.y);

        // This is doing a slight roll which is then passed to the Z rotation axis
        _currentRollAngle = Mathf.Lerp(_currentRollAngle, Input.GetAxisRaw("Mouse X") * _rollAngle, Time.deltaTime * _rollSpeed);

        // Apply rotation on the X axis to look up and down and apply the slight roll on the Z axis.
        // This is done to the LookRoot which holds the cameras.
        _lookRoot.localRotation = Quaternion.Euler(_lookAngles.x, 0f, _currentRollAngle);
        // Apply rotation to the Y axis to look left and right to the player root body.
        _playerRoot.localRotation = Quaternion.Euler(0f, _lookAngles.y, 0f);
    }
}