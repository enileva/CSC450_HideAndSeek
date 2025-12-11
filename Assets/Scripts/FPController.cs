using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PurrNet;

[RequireComponent(typeof(CharacterController))]
public class FPSController : NetworkBehaviour
{
    public Camera playerCamera;
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float jumpPower = 3f;
    public float gravity = 10f;


    public float lookSpeed = 2f;
    public float lookXLimit = 45f;


    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;

    public bool canMove = true;


    public GameObject _quiet;
    public GameObject _loud;
    public bool isWalking = false;
    public bool isJumping = false;

    public Vector3 deadZonePosition = new Vector3(9999, -9999, 9999);

    public void KillLocalPlayer()
    {
        // only the owner should try to move / disable its own body
        if (!isOwner) return;

        canMove = false;

        // Disable CharacterController while we teleport so it doesn't fight us
        if (characterController != null)
            characterController.enabled = false;

        transform.position = deadZonePosition;

        // Re-enable so it doesn’t stay broken in case of respawn later
        if (characterController != null)
            characterController.enabled = true;
    }

    CharacterController characterController;
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        if (isOwner)
        {

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            _quiet = GameObject.Find("Player/QuietNoise");
            _loud = GameObject.Find("Player/LoudNoise");
        }
        if (!isOwner)
        {

            if (playerCamera != null)
            {
                playerCamera.enabled = false;
                var listener = playerCamera.GetComponent<AudioListener>();
                if (listener) listener.enabled = false;
            }
        }
    }

    void OnDestroy()
    {
        if (isOwner)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void Update()
    {
        if (!isOwner) return;

        #region Handles Movment
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        // Press Left Shift to run
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float curSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);
        if (curSpeedX != 0 || curSpeedY != 0) isWalking = true;
        else isWalking = false;

        #endregion

        #region Handles Jumping
        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpPower;
            isJumping = true;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        #endregion

        #region Handles Rotation
        characterController.Move(moveDirection * Time.deltaTime);

        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }

        #endregion

        #region Handles Noises
        if (_quiet != null && _loud != null)
        {

            if (characterController.isGrounded)
            {
                if (isJumping || (isRunning && isWalking))
                {
                    isJumping = false;
                    // Enable loud noise
                    _loud.SetActive(true);
                    _quiet.SetActive(false);
                }
                else if (isWalking)
                {
                    // Enable quiet noise
                    _quiet.SetActive(true);
                    _loud.SetActive(false);
                }
                else
                {
                    _quiet.SetActive(false);
                    _loud.SetActive(false);
                }
            }
            else
            {
                _loud.SetActive(false);
                _quiet.SetActive(false);
            }
        }

        #endregion
    }
}
