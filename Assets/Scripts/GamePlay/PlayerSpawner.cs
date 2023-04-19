using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class PlayerSpawner : NetworkBehaviour
{
    [SerializeField] private List<Vector2> _spawnPoints;
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private float _countdownLength;

    [Header("Player Connections (Networked Variables)")]
    [SerializeField] private NetworkVariable<int> _playersInLobby = new NetworkVariable<int>();
    [SerializeField] private NetworkVariable<int> _connectedPlayers = new NetworkVariable<int>();
    [SerializeField] private NetworkVariable<float> _countdown = new NetworkVariable<float>(3.0f);

    [Header("Unity Events")]
    public UnityEvent onAllPlayersConnected;
    public UnityEventDoubleInt onPlayerConnected;
    public UnityEventFloat onCountdownTickDown;
    public UnityEvent onCountdownOver;
    
    public override void OnNetworkSpawn()
    {
        Debug.LogError("ON NETWORK SPAWN CALLED");
        _connectedPlayers.OnValueChanged += UpdateConnectedPlayers;
        _countdown.OnValueChanged += RefreshCountdownUI;
        
        if (IsServer)
        {
            Debug.Log("Spawned PlayerSpawner");
            onAllPlayersConnected.AddListener(SpawnPlayers);
            _playersInLobby.Value = ManagerSystems.Instance.GetNetworkingManager().GetLobbyManager().GetConnectedPlayerCount();
            _connectedPlayers.Value += 1;
        }
        else
        {
            // UpdateConnectedPlayers(_playersInLobby.Value, _connectedPlayers.Value);
            // TODO check if game already running
            RPC_CheckInPlayerServerRPC();
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
            player.GetComponent<NetworkObject>().SpawnAsPlayerObject(client);
            index++;
        }
    }
}
