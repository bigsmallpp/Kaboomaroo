using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour
{
    public Rigidbody2D rigidbody_player { get; private set; }
    public float speed = 3f;

    private CustomInput input = null;
    private Vector2 direction = Vector2.zero;
    private void Awake()
    {
        rigidbody_player = GetComponent<Rigidbody2D>();
        input = new CustomInput();
        input.Player.Movement.performed += OnMovementPerformed;
        input.Player.Movement.canceled += OnMovementStopped;
    }
    private void FixedUpdate()
    {
        // TODO Only for testing
        return;
        if (IsOwner && IsServer)
        {
            rigidbody_player.velocity = direction * speed;
            Debug.Log("Moving in Owner + Server");
        }
        else if (IsOwner && IsClient && !IsServer)
        {
            rigidbody_player.velocity = direction * speed;
            
            // EFFICIENCY
            if (rigidbody_player.velocity == Vector2.zero && direction == Vector2.zero)
            {
                return;
            }
            RPC_SetPlayerVelocityServerRpc(direction, speed);
            Debug.Log("Moving in Owner + Client");
        }

    }

    private void OnEnable()
    {
        input.Enable();
        input.Player.Movement.performed += OnMovementPerformed;
        input.Player.Movement.canceled += OnMovementStopped;
        input.CheckForButton.CheckForButtonAction.performed += OnButtonPressed;
    }

    private void OnDisable()
    {
        input.Disable();
        input.Player.Movement.performed -= OnMovementPerformed;
        input.Player.Movement.canceled -= OnMovementStopped;
        input.CheckForButton.CheckForButtonAction.performed -= OnButtonPressed;
    }

    private void OnMovementPerformed(InputAction.CallbackContext value)
    {
        direction = value.ReadValue<Vector2>();
    }

    private void OnMovementStopped(InputAction.CallbackContext value)
    {
        direction = Vector2.zero;
    }
   
    private void OnButtonPressed(InputAction.CallbackContext context) //Template for adding listener on buttons and actions
    {
        if (context.action.triggered && context.action.ReadValue<float>() != 0 && context.action.phase == InputActionPhase.Performed)
        {
            //Perform Trigger Pressed Actions
            Debug.Log("Button Pressed");
        }
    }

    [ServerRpc]
    private void RPC_SetPlayerVelocityServerRpc(Vector2 velocity, float speed)
    {
        rigidbody_player.velocity = velocity * speed;
    }
}
    