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
    public bool _alive = false;

    public UnityEventBool onMovementPerformed;
    public UnityEventBool onMovementStopped;

    private CustomInput input = null;
    private Vector2 direction = Vector2.zero;
    
    private NetworkVariable<bool> _controlsEnabled = new NetworkVariable<bool>(true);
    [SerializeField] private bool _colliding = false;

    [Header("Networked Vars")] [SerializeField]
    private NetworkVariable<Vector2> _directionNetworked = new NetworkVariable<Vector2>(new Vector2(0, 0),
                                                    NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<float> _speedNetworked = new NetworkVariable<float>(3.0f, 
                                                    NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [SerializeField]
    private NetworkVariable<int> _bombRadius = new NetworkVariable<int>(1,
                                                    NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [SerializeField]
    private NetworkVariable<int> _bombCount = new NetworkVariable<int>(2,
                                                NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    //[SerializeField]
    //private NetworkVariable<int> _activeBombs = new NetworkVariable<int>(0,
    //                                        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private int _activeBombs = 0;

    [SerializeField]
    public NetworkVariable<int> skin_variant = new NetworkVariable<int>(1);

    [Header("Bomb Prefab")]
    [SerializeField] private GameObject _prefBomb;

    private AnimUpdater animUpdater;

    public BombCountScript bombCountScript;

    public enum Direction
    {
        Up = 1,
        Down = 2,
        Left = 3,
        Right = 4,
        Idle = 5
    }

    Direction curr_direction;

    private void Awake()
    {
        rigidbody_player = GetComponent<Rigidbody2D>();
        input = new CustomInput();
        input.Player.Movement.performed += OnMovementPerformed;
        input.Player.Movement.canceled += OnMovementStopped;
        input.Player.Actions.performed += PlantBomb;
        animUpdater = GetComponentInChildren<AnimUpdater>();
        initBombCountScriptObject();
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
        
        if(IsOwner)
        {
            animUpdater.updateAnim(curr_direction);
            if (bombCountScript == null)
            {
                initBombCountScriptObject();
            }
            else
            {
                bombCountScript.UpdateBombCount(_bombCount.Value - _activeBombs);
            }
        }
        
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

        onMovementPerformed.Invoke(true);

        direction = direction.normalized;
        setCurrentDirection(direction);
        
    }

    private void setCurrentDirection(Vector2 direction)
    {
        if (Math.Abs(direction.x) > Math.Abs(direction.y))
        {
            if (direction.x > 0)
            {
                curr_direction = Direction.Right;
            }
            else
            {
                curr_direction = Direction.Left;
            }
        }
        else
        {
            if (direction.y > 0)
            {
                curr_direction = Direction.Up;
            }
            else
            {
                curr_direction = Direction.Down;
            }
        }
    }

    private void OnMovementStopped(InputAction.CallbackContext value)
    {
        direction = Vector2.zero;
        curr_direction = Direction.Idle;
        onMovementStopped.Invoke(true);
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
            int radius = _bombRadius.Value;
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
        if (colliders.Length > 0)
        {
            foreach(Collider2D obj in colliders)
            {
                if (obj.gameObject.TryGetComponent(out Bomb current_bomb) || owner.GetComponent<PlayerController>()._bombCount.Value <= owner.GetComponent<PlayerController>()._activeBombs )
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
        owner.GetComponent<PlayerController>()._activeBombs++;
    }

    public void DecreaseActiveBombs(GameObject owner)
    {
        this.GetComponent<PlayerController>()._activeBombs--;
        //owner.GetComponent<PlayerController>()._activeBombs.Value--;
    }

    [ServerRpc]
    private void RPC_RequestSpawnBombServerRpc(ulong owner, int radius)
    {
        GameObject owner_object = GameObject.FindWithTag("PlayerSpawner").GetComponent<PlayerSpawner>().GetPlayerOfClient(owner);
        SpawnBomb(owner_object, radius);
    }

    [ClientRpc]
    public void killPlayerClientRpc()
    {
        animUpdater.animPlayerDead();
    }

    public void increaseRadiusItem(bool used)
    {
        if (!used)
        {
            _bombRadius.Value++;
        }
    }

    public void increaseSpeedItem(bool used)
    {
        if (!used)
        {
            speed += 1.0f;
        }
    }

    public void increaseBombLimitItem(bool used)
    {
        if (!used)
        {
            Debug.Log("Bomblimit Upgrade used");
            _bombCount.Value++;
        }
    }
    [ClientRpc]
    public void setSpeedClientRpc(float new_speed)
    {
        Debug.Log("Set speed client rpc");
        speed = new_speed;
        rigidbody_player.velocity = direction * speed;
    }

    public void setAliveStatus(bool isAlive)
    {
        _alive = isAlive;
    }

    public bool getAliveStatus()
    {
        return _alive;
    }


    [ClientRpc]
    public void RPC_gameFinishedClientRPC()
    {
        // SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene("MainMenuDisconnect", LoadSceneMode.Single);
    }

    private void initBombCountScriptObject()
    {
        bombCountScript = GameObject.FindObjectOfType<BombCountScript>();
    }
}
    