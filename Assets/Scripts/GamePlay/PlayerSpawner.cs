using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Networking.Transport;
using Unity.Services.Lobbies.Models;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PlayerSpawner : NetworkBehaviour
{
    [SerializeField] private List<Vector2> _spawnPoints;
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private float _countdownLength;
    [SerializeField] private float _timerForPlayersToConnect = 10.0f;
    [SerializeField] private bool _gameStarted = false;
    [SerializeField] private bool _gameFinished = false;

    [Header("Player Connections (Networked Variables)")]
    [SerializeField] private NetworkVariable<int> _playersInLobby = new NetworkVariable<int>();
    [SerializeField] private NetworkVariable<int> _connectedPlayers = new NetworkVariable<int>();
    [SerializeField] private NetworkVariable<float> _countdown = new NetworkVariable<float>(3.0f);

    [Header("Unity Events")]
    public UnityEvent onAllPlayersConnected;
    public UnityEventDoubleInt onPlayerConnected;
    public UnityEventFloat onCountdownTickDown;
    public UnityEvent onCountdownOver;

    [Header("The Players")]
    [SerializeField] private List<Tuple<ulong, GameObject>> _players = new List<Tuple<ulong, GameObject>>();

    
    public override void OnNetworkSpawn()
    {
        Debug.LogError("ON NETWORK SPAWN CALLED");
        _connectedPlayers.OnValueChanged += UpdateConnectedPlayers;
        _countdown.OnValueChanged += RefreshCountdownUI;
        
        if (IsServer)
        {
            Debug.Log("Spawned PlayerSpawner");
            onAllPlayersConnected.AddListener(SpawnPlayers);
            onAllPlayersConnected.AddListener(RemoveRelayCode);
            _playersInLobby.Value = LobbyManager.Instance.GetConnectedPlayerCount();
            _connectedPlayers.Value += 1;
        }
        else
        {
            // UpdateConnectedPlayers(_playersInLobby.Value, _connectedPlayers.Value);
            // TODO check if game already running
            RPC_CheckInPlayerServerRPC();
        }
    }

    private void Update()
    {
        ReduceTimerForPlayersToConnect();
        if(_gameStarted)
            CheckPlayerAliveStatus();
        if (!_gameStarted && _gameFinished)
        {
            StartCoroutine(returnAllPlayersToMenu());
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RPC_CheckInPlayerServerRPC()
    {
        Debug.Log("RPC Received");
        _connectedPlayers.Value += 1;
    }

    public void UpdateConnectedPlayers(int previous, int current)
    {
        onPlayerConnected.Invoke(new DoubleInt(current, _playersInLobby.Value));
        
        if (current > 0 && current == _playersInLobby.Value)
        {
            _connectedPlayers.OnValueChanged -= UpdateConnectedPlayers;
            onAllPlayersConnected.Invoke();
            
            onPlayerConnected.RemoveAllListeners();
            onAllPlayersConnected.RemoveAllListeners();

            if (IsServer)
            {
                StartCoroutine(StartCountdown());
            }
        }
        
        Debug.Log("Update Connected Players called");
    }

    public DoubleInt GetConnectedPlayerCount()
    {
        return new DoubleInt(_connectedPlayers.Value, _playersInLobby.Value);
    }

    public void RefreshCountdownUI(float previous, float current)
    {
        onCountdownTickDown.Invoke(current);
        if (current == 0.0f)
        {
            onCountdownOver.Invoke();
            onCountdownTickDown.RemoveAllListeners();
            onCountdownOver.RemoveAllListeners();
            _gameStarted = true;
            // TODO Give Players Authority
        }
    }

    private IEnumerator StartCountdown()
    {
        float elapsed = 0.0f;
        while (elapsed <= _countdownLength)
        {
            elapsed += Time.deltaTime;
            
            if (_countdownLength - elapsed < 0)
            {
                elapsed = _countdownLength;
            }
            _countdown.Value = _countdownLength - elapsed;
            yield return null;
        }

    }

    private void SpawnPlayers()
    {
        IReadOnlyList<ulong> ids = NetworkManager.Singleton.ConnectedClientsIds;

        int index = 0;
        foreach (ulong client in ids)
        {
            GameObject player = Instantiate(_playerPrefab, _spawnPoints[index], Quaternion.identity);
            player.GetComponent<PlayerController>().skin_variant.Value = index + 1;
            player.GetComponent<NetworkObject>().SpawnAsPlayerObject(client);
            player.GetComponent<PlayerController>().DisableControls();
            onCountdownOver.AddListener(player.GetComponent<PlayerController>().EnableControls);
            player.GetComponent<PlayerController>().setAliveStatus(true);
            _players.Add(new Tuple<ulong, GameObject>(client, player));
            
            index++;
        }
    }

    public void DespawnPlayer(ulong clientId)
    {
        Debug.LogError("Despawn Player Called");
        Tuple<ulong, GameObject> playerTuple = _players.Find(player => player.Item1 == clientId);

        if (playerTuple != null)
        {
            GameObject player = playerTuple.Item2;
            _players.Remove(playerTuple);
            player.GetComponent<PlayerController>().setAliveStatus(false);
            player.GetComponent<NetworkObject>().Despawn();
            Destroy(player);
            Debug.Log("Despawned the player, which quitted!");
        }
        
    }

    public override void OnNetworkDespawn()
    {
        Debug.LogError("ON NETWORK DESPAWN CALLED");
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene("MainMenuDisconnect", LoadSceneMode.Single);
    }

    public void RemoveRelayCode()
    {
        LobbyManager.Instance.ResetRelayCode();
    }

    public GameObject GetPlayerOfClient(ulong owner)
    {
        foreach (Tuple<ulong, GameObject> entry in _players)
        {
            if (entry.Item1 == owner)
            {
                return entry.Item2;
            }
        }
        
        Debug.LogError("No according PlayerObject found");
        return null;
    }

    private void ReduceTimerForPlayersToConnect()
    {
        if (!IsServer || _timerForPlayersToConnect <= 0)
        {
            return;
        }

        _timerForPlayersToConnect -= Time.deltaTime;
        if (_timerForPlayersToConnect <= 0)
        {
            _timerForPlayersToConnect = 0.0f;
            // Force Game Start
            onAllPlayersConnected.Invoke();
        }
    }

    private void CheckPlayerAliveStatus()
    {
        int count = 0;
        GameObject lastManStanding = null;
        foreach(var player in _players)
        {
            if(player.Item2.gameObject.GetComponent<PlayerController>().getAliveStatus() == true)
            {
                count++;
                lastManStanding = player.Item2.gameObject;
            }
        }
        if(count == 1)
        {
            //ToDo: Show End Screen! Return to Lobby Screen after XX Seconds!
            lastManStanding.GetComponent<PlayerController>().DisableControls();
            Debug.Log("Last man Standing!");
            Debug.Log("Player: " + lastManStanding.GetComponent<NetworkObject>().OwnerClientId + " won!");
            _gameFinished = true;
            _gameStarted = false;
            GameObject.FindWithTag("NetworkedMenuManager").GetComponent<NetworkedGameMenus>().RPC_SwitchToWinnerMessageClientRPC(lastManStanding.GetComponent<NetworkObject>().OwnerClientId);
            //NetworkManager.Singleton.DisconnectClient(lastManStanding.GetComponent<NetworkObject>().OwnerClientId);
        }
    }

    private IEnumerator returnAllPlayersToMenu()
    {
        yield return new WaitForSecondsRealtime(3);
        foreach(var player in _players)
        {
            player.Item2.gameObject.GetComponent<PlayerController>().RPC_gameFinishedClientRPC();
        }

        if (IsServer)
        {
            NetworkManager.Singleton.Shutdown();
            SceneManager.LoadScene("MainMenuDisconnect", LoadSceneMode.Single);
        }
    }
    
}
