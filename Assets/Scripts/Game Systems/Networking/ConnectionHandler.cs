using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Networking.Transport;
using Unity.Services.Authentication;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConnectionHandler : MonoBehaviour
{
    private PlayerSpawner _playerSpawner;
    private bool _isServer;
    
    private void Start()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnect;
        NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnect;
        _isServer = NetworkManager.Singleton.IsServer;
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton == null)
        {
            return;
        }
        
        NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnect;
        NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnect;
    }

    public void HandleClientDisconnect(ulong clientID)
    {
        // Client -> Host dc'd
        if (!_isServer && (clientID == _playerSpawner._hostID.Value || _playerSpawner._hostID.Value == 0))
        {
            if (_playerSpawner._plannedShutdown.Value == true)
            {
                return;
            }
            // Cleanup for Host and Client
            NetworkManager.Singleton.Shutdown();
            LobbyManager.Instance.InvalidateLobby();
            Debug.LogError("HandleClientDisconnect");
            SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
            return;
        }
        
        // Host -> other client dc'd
        if (_isServer && clientID != NetworkManager.Singleton.LocalClientId)
        {
            // Server is always lobby host
            _playerSpawner.RemovePlayer(clientID);
            return;
        }
    }
    
    public void HandleClientConnect(ulong clientID)
    {
        
    }

    public void SetPlayerSpawner(PlayerSpawner spawner)
    {
        _playerSpawner = spawner;
    }
}
