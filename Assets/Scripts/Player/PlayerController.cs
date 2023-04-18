using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D rigidbody_player { get; private set; }
    public Vector2 direction = Vector2.down;
    public float speed = 3f;

    private CustomInput input = null;
    private Vector2 move = Vector2.zero;
    private void Awake()
    {
        rigidbody_player = GetComponent<Rigidbody2D>();
        input = new CustomInput();
        input.Player.Movement.performed += OnMovementPerformed;
        input.Player.Movement.canceled += OnMovementStopped;
    }
    private void FixedUpdate()
    {
        rigidbody_player.velocity = move * speed;
        //Debug.Log("moving");
        //Debug.Log(move);
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
        move = value.ReadValue<Vector2>();
    }

    private void OnMovementStopped(InputAction.CallbackContext value)
    {
        move = Vector2.zero;
    }
   
    private void OnButtonPressed(InputAction.CallbackContext context) //Template for adding listener on buttons and actions
    {
        if (context.action.triggered && context.action.ReadValue<float>() != 0 && context.action.phase == InputActionPhase.Performed)
        {
            //Perform Trigger Pressed Actions
            Debug.Log("Button Pressed");
        }
    }
}
    