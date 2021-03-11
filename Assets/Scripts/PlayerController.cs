using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private Player playerInput;
    private Transform cameraMain;
    private PlayerState playerState = PlayerState.Walking;
    private int currentTeleport;
    private int previousTeleport;

    enum PlayerState { Teleporting, Walking, Idle, Jumping};

    [SerializeField]
    private float playerSpeed = 2.0f;
    [SerializeField]
    private float jumpHeight = 1.0f;
    [SerializeField]
    private float gravityValue = -9.81f;
    [SerializeField]
    GameObject teleportLocation;
    [SerializeField]
    Animation fadePanel;

    private void Awake()
    {
        Assert.IsNotNull(teleportLocation);
        playerInput = new Player();
        controller = gameObject.GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        playerInput.Enable();
    }

    private void OnDisable()
    {
        playerInput.Disable();
    }

    private void Start()
    {
        previousTeleport = 0;
        cameraMain = Camera.main.transform;
    
    }

    void Update()
    {
        if(playerState != PlayerState.Teleporting)
            CharacterMovement();
        TeleportPlayer();
    }

    void TeleportPlayer()
    {
        if (playerInput.PlayerMain.Teleport.triggered && playerState != PlayerState.Teleporting)
        {
            playerState = PlayerState.Teleporting;
            fadePanel.Play("FadePanel");
            currentTeleport = UnityEngine.Random.Range(0, teleportLocation.transform.childCount);
            if (currentTeleport >= previousTeleport)
                currentTeleport = (currentTeleport + 1) % teleportLocation.transform.childCount;
            Vector3 randomChild = teleportLocation.transform.GetChild(currentTeleport).transform.position;
            previousTeleport = currentTeleport;
            transform.position = new Vector3(randomChild.x, randomChild.y, randomChild.z);
            StartCoroutine(TeleportLag());
            
        }
    }

    IEnumerator TeleportLag()
    {
        yield return new WaitForSeconds(2);
        playerState = PlayerState.Idle;
    }


    void CharacterMovement()
    {
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        Vector2 movementInput = playerInput.PlayerMain.Move.ReadValue<Vector2>();
        Vector3 move = (cameraMain.forward * movementInput.y + cameraMain.right * movementInput.x);
        move.y = 0f;
        controller.Move(move.normalized * Time.deltaTime * playerSpeed);

        if (move != Vector3.zero)
        {
            playerState = PlayerState.Walking;
            gameObject.transform.forward = move;
        }

        // Changes the height position of the player..
        if (playerInput.PlayerMain.Jump.triggered && groundedPlayer)
        {
            playerState = PlayerState.Jumping;
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        /* if(movementInput != Vector2.zero)
         {
             Quaternion rotation = Quaternion.Euler(new Vector3(child.localEulerAngles.x, cameraMain.localEulerAngles.y, child.localEulerAngles.z));
             child.rotation = Quaternion.Lerp(child.rotation, rotation, Time.deltaTime * rotationSpeed);
         }*/
    }
}
