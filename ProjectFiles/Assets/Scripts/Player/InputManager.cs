using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.VirtualTexturing;
using Photon.Pun;
using Photon.Realtime;

public class InputManager : MonoBehaviourPun
{
    private PlayerInput playerInput;
    public PlayerInput.OnFootActions onFoot;
    private PlayerMotor motor;
    private PlayerLook look;
    [SerializeField]
    private GunSystem gunSystem;
    public bool EnableSprint = false;
    private bool isShooting = false;
    private bool Dead;
    public GameObject childObject;
    private float rotX;
    private float rotY;
    void Awake()
    {
        playerInput = new PlayerInput();
        onFoot = playerInput.OnFoot;
        motor = GetComponent<PlayerMotor>();
        look = GetComponent<PlayerLook>();
        onFoot.Jump.performed += ctx => motor.Jump();
        if (EnableSprint)
        {
            onFoot.Sprint.started += ctx => motor.Sprint(true);
            onFoot.Sprint.canceled += ctx => motor.Sprint(false);
        }
        onFoot.Attack.started += ctx =>
        {
            isShooting = true;
            gunSystem.holding = true;
        };
        onFoot.Attack.canceled += ctx =>
        {
            isShooting = false;
            gunSystem.holding = false;
        };

        onFoot.Aim.started += ctx => gunSystem.OnAimStatusChange(true);
        onFoot.Aim.canceled += ctx => gunSystem.OnAimStatusChange(false);

        onFoot.Reload.performed += ctx => gunSystem.Reload();
        Dead = GetComponent<PlayerStatus>().isDead;
    }
    private void Update()
    {
        
    }
    void FixedUpdate()
    {
        
    }
    private void LateUpdate()
    {
        if (!photonView.IsMine || Dead)
        {
            // we'll handle movement for other players via the PhotonTransformView, so just return if this player isn't me
            return;
        }

        if (isShooting)
        {
            gunSystem.TryShoot();
        }
        motor.ProcessMove(onFoot.Movement.ReadValue<Vector2>());
        look.ProcessLook(onFoot.Look.ReadValue<Vector2>());
    }

    private void OnEnable()
    {
        onFoot.Enable();
    }
    private void OnDisable()
    {
        onFoot.Disable();
    }

}
