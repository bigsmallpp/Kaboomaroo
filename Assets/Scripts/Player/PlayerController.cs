using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : NetworkBehaviour
{
    public Rigidbody2D rigidbody_player { get; private set; }
    public float speed = 3f;

    private CustomInput input = null;
    private Vector2 direction = Vector2.zero;
    
    private NetworkVariable<bool> _controlsEnabled = new NetworkVariable<bool>(true);
    [SerializeField] private bool _colliding = false;

    [Header("Networked Vars")] [SerializeField]
    private NetworkVariable<Vector2> _directionNetworked = new NetworkVariable<Vector2>(new Vector2(0, 0),
                                                    NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<float> _speedNetworked = new NetworkVariable<float>(3.0f, 
                                                    NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [Header("Bomb Prefab")]
    [SerializeField] private GameObject _prefBomb;

    private void Awake()
    {
        rigidbody_player = GetComponent<Rigidbody2D>();
        input = new CustomInput();
        input.Player.Movement.performed += OnMovementPerformed;
        input.Player.Movement.canceled += OnMovementStopped;
        input.Player.Actions.performed += PlantBomb;
        
    }
    private void FixedUpdate()
    {
        if (!_controlsEnabled.Value)
        {
            return;
        }

        if (IsOwner)
        {
            _directionNetworked.Value = direction;
            _speedNetworked.Value = speed;
        }

        rigidbody_player.velocity = direction * speed;
    }

    private void OnEnable()
    {
        input.Enable();
        input.Player.Movement.performed += OnMovementPerformed;
        input.Player.Movement.canceled += OnMovementStopped;
        
        // Interaction Key (Space) For Bombas
        input.Player.Actions.performed += PlantBomb;
        
        // Escape Key for Menus
        input.CheckForButton.CheckForButtonAction.performed += OnButtonPressed;
    }

    private void OnDisable()
    {
        input.Disable();
        input.Player.Movement.performed -= OnMovementPerformed;
        input.Player.Movement.canceled -= OnMovementStopped;
        
        // Interaction Key (Space) For Bombas
        input.Player.Actions.performed -= PlantBomb;
        
        // Escape Key for Menus
        input.CheckForButton.CheckForButtonAction.performed -= OnButtonPressed;
    }

    private void OnMovementPerformed(InputAction.CallbackContext value)
    {
        direction = value.ReadValue<Vector2>();
        direction = direction.normalized;
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
            Debug.Log("Escape Pressed");
        }
    }
    
    private void PlantBomb(InputAction.CallbackContext context)
    {
        if (!_controlsEnabled.Value || !IsOwner)
        {
            return;
        }

        if (context.action.triggered && context.action.ReadValue<float>() != 0 && context.action.phase == InputActionPhase.Performed)
        {
            int radius = 1;
            // TODO Check whether there's already a bomb in place
            if (IsServer)
            {
                // TODO Exchange with item boosts
                SpawnBomb(gameObject, radius);
            }
            else
            {
                RPC_RequestSpawnBombServerRpc(OwnerClientId, radius);
            }
        }
    }

    public void EnableControls()
    {
        _controlsEnabled.Value = true;
    }
    
    public void DisableControls()
    {
        _controlsEnabled.Value = false;
    }

    private void SpawnBomb(GameObject owner, int radius)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(owner.transform.position, 0.5f);
        if(colliders.Length > 0)
        {
            foreach(Collider2D obj in colliders)
            {
                if (obj.gameObject.TryGetComponent(out Bomb current_bomb))
                {
                    // Can't spawn a bomb on top of a bomb
                    return;
                }
            }
        }
        
        Vector3Int pos = Vector3Int.RoundToInt(owner.transform.position);
        GameObject bomb = Instantiate(_prefBomb, pos, Quaternion.identity);
        bomb.GetComponent<Bomb>().SetOwner(owner);
        bomb.GetComponent<Bomb>().SetRadius(radius);
        bomb.GetComponent<NetworkObject>().Spawn();
    }

    [ServerRpc]
    private void RPC_RequestSpawnBombServerRpc(ulong owner, int radius)
    {
        GameObject owner_object = GameObject.FindWithTag("PlayerSpawner").GetComponent<PlayerSpawner>().GetPlayerOfClient(owner);
        SpawnBomb(owner_object, radius);
    }
}
    