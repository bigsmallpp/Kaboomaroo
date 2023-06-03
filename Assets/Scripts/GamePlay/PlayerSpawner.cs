using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Networking.Transport;
using Unity.Services.Authentication;
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
    [SerializeField] public NetworkVariable<ulong> _hostID = new NetworkVariable<ulong>(0);

    [Header("Unity Events")]
    public UnityEvent onAllPlayersConnected;
    public UnityEventDoubleInt onPlayerConnected;
    public UnityEventFloat onCountdownTickDown;
    public UnityEvent onCountdownOver;

    [Header("The Players")]
    [SerializeField] private List<Tuple<ulong, GameObject>> _players = new List<Tuple<ulong, GameObject>>();
    [SerializeField] private List<Tuple<ulong, string>> _lobbyIDs = new List<Tuple<ulong, string>>();

    
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
            _hostID.Value = NetworkManager.Singleton.LocalClientId;
        }
        else
        {
            // UpdateConnectedPlayers(_playersInLobby.Value, _connectedPlayers.Value);
            // TODO check if game already running
            RPC_CheckInPlayerServerRPC(NetworkManager.Singleton.LocalClientId, AuthenticationService.Instance.PlayerId);
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
    private void RPC_CheckInPlayerServerRPC(ulong clientID, string lobbyID)
    {
        Debug.Log("RPC Received");
        _connectedPlayers.Value += 1;
        _lobbyIDs.Add(new Tuple<ulong, string>(clientID, lobbyID));
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

    public override void OnNetworkDespawn()
    {
        Debug.LogError("ON NETWORK DESPAWN CALLED");
        // NetworkManager.Singleton.Shutdown();
        // SceneManager.LoadScene("MainMenuDisconnect", LoadSceneMode.Single);
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
        // TODO Only for testing
        return;
        
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

    public void RemovePlayer(ulong clientID)
    {
        Tuple<ulong, GameObject> player_to_remove = null;
        foreach (Tuple<ulong, GameObject> player in _players)
        {
            if (player.Item1 == clientID)
            {
                player_to_remove = player;
                if (player.Item2 != null)
                {
                    player.Item2.gameObject.GetComponent<PlayerController>().setAliveStatus(false);
                }

                break;
            }
        }

        if (player_to_remove != null)
        {
            _players.Remove(player_to_remove);
        }
        
        Tuple<ulong, string> lobby_entry_to_remove = null;
        foreach (Tuple<ulong, string> player in _lobbyIDs)
        {
            if (player.Item1 == clientID)
            {
                lobby_entry_to_remove = player;
                break;
            }
        }

        if (lobby_entry_to_remove != null)
        {
            _lobbyIDs.Remove(lobby_entry_to_remove);
        }
    }

    public string GetLobbyIdOfPlayer(ulong clientID)
    {
        foreach (Tuple<ulong, string> player in _lobbyIDs)
        {
            if (player.Item1 == clientID)
            {
                return player.Item2;
            }
        }

        return string.Empty;
    }
    
}
