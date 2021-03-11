using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(CinemachineFreeLook))]
public class CameraLook : MonoBehaviour
{

    [SerializeField]
    float lookSpeed = 2f;

    private CinemachineFreeLook cinemachine;
    private Player playerInput;

    private void Awake()
    {
        playerInput = new Player();
        cinemachine = GetComponent<CinemachineFreeLook>();
    }

    private void OnEnable()
    {
        playerInput.Enable();
    }

    private void OnDisable()
    {
        playerInput.Disable();
    }


    void Update()
    {

        Vector2 delta = playerInput.PlayerMain.Move.ReadValue<Vector2>();
        // Debug.Log("detla x: " + delta.x + "\n delta y: " + delta.y);
        cinemachine.m_XAxis.Value += delta.x * lookSpeed * Time.deltaTime;
        cinemachine.m_YAxis.Value += delta.y * lookSpeed * Time.deltaTime + 50f;

    }
}
