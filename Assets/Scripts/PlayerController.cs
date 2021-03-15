using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
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
    private Transform child;
    private AudioSource audioSource;
    private Animator charAnimator;

    enum PlayerState { Teleporting, Walking, Idle, Jumping, Spraying};

    [Header("Player Control")]
    [SerializeField]
    private float playerSpeed = 2.0f;
    [SerializeField]
    private float jumpHeight = 1.0f;
    [SerializeField]
    private float gravityValue = -9.81f;
    [SerializeField]
    private float rotationSpeed = 4f;
    [Space(10)]

    [Header("Extra Stuff")]
    [SerializeField]
    GameObject teleportLocation;
    [SerializeField]
    Animation fadePanel;
    [SerializeField]
    LayerMask mazeLayer;
    [SerializeField]
    GameObject graffitiArrow;
    [Space(10)]

    [Header("Sound")]
    [SerializeField]
    AudioClip graffitiSound;
    [SerializeField]
    AudioClip TeleportingSound;

    private void Awake()
    {
        Assert.IsNotNull(teleportLocation);
        playerInput = new Player();
        controller = gameObject.GetComponent<CharacterController>();
        audioSource = gameObject.GetComponent<AudioSource>();
        charAnimator = gameObject.transform.GetChild(0).GetComponent<Animator>();
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
        child = transform.GetChild(0).transform;
        SceneManager.LoadSceneAsync("UI", LoadSceneMode.Additive);
    }

    void Update()
    {
        if(playerState != PlayerState.Teleporting)
            CharacterMovement();
        TeleportPlayer();
        if (playerInput.PlayerMain.Spray.triggered)
        {
            TagWall();
        }
        
    }

    private void TagWall()
    {
        RaycastHit hit = new RaycastHit();
        Vector3 directionRay = transform.rotation * Vector3.forward;
        Ray ray = new Ray(transform.position, directionRay);

        if(Physics.Raycast(ray, out hit, 3, mazeLayer))
        {
            var hitRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            Vector3 hitPoint;
            hitPoint = hit.point + hit.normal * 0.1f;
            hitPoint.y += 1f;
            Instantiate(graffitiArrow, hitPoint, hitRotation);
            audioSource.PlayOneShot(graffitiSound);
        }
    }

    void TeleportPlayer()
    {
        if (playerInput.PlayerMain.Teleport.triggered && playerState != PlayerState.Teleporting)
        {
            charAnimator.SetBool("Moving", false);
            playerState = PlayerState.Teleporting;
            fadePanel.Play("FadePanel");
            currentTeleport = UnityEngine.Random.Range(0, teleportLocation.transform.childCount);
            if (currentTeleport == previousTeleport)
                currentTeleport = (currentTeleport + 1) % teleportLocation.transform.childCount;
            Vector3 randomChild = teleportLocation.transform.GetChild(currentTeleport).transform.position;
            previousTeleport = currentTeleport;
            transform.position = new Vector3(randomChild.x, randomChild.y, randomChild.z);
            audioSource.PlayOneShot(TeleportingSound);
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
            charAnimator.SetBool("Moving", true);
            gameObject.transform.forward = move;
        }
        else
        {
            charAnimator.SetBool("Moving", false);
        }

        // Changes the height position of the player..
        if (playerInput.PlayerMain.Jump.triggered && groundedPlayer)
        {
            playerState = PlayerState.Jumping;
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        if(movementInput != Vector2.zero)
         {
             Quaternion rotation = Quaternion.Euler(new Vector3(child.localEulerAngles.x, cameraMain.localEulerAngles.y, child.localEulerAngles.z));
             child.rotation = Quaternion.Lerp(child.rotation, rotation, Time.deltaTime * rotationSpeed);
         }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Trap")
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        if(other.tag == "Prize")
        {
            SceneManager.LoadScene("WinScreen");
        }
    }
}
