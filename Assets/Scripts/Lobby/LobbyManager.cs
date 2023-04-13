using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{

    private Lobby _connectedLobby;
    
    [Header("Heartbeat Variables")]
    [SerializeField] private float _heartbeatElapsedTime = 0.0f;
    [SerializeField] private float _heartbeatInterval = 20.0f;
    
    [Header("Lobby Update Variables")]
    [SerializeField] private float _lobbyUpdateElapsedTime = 0.0f;
    [SerializeField] private float _lobbyUpdateInterval = 2.5f;
    private async void Start()
    {
        DontDestroyOnLoad(this);
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += SignedIn;
        AuthenticationService.Instance.SignInFailed += SignInFailed;
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    private void Update()
    {
        KeepLobbyAlive();
        UpdateLobbyData();
    }

    private void SignedIn()
    {
        Debug.Log("Signed In Player " + AuthenticationService.Instance.PlayerId);
    }

    private void SignInFailed(RequestFailedException e)
    {
        Debug.LogError("e");
    }
    
    public async Task<string> CreateLobby(string name, int max_players)
    {
        try
        {
            // TODO Read Docs
            CreateLobbyOptions options = new CreateLobbyOptions();
            
            _connectedLobby = await LobbyService.Instance.CreateLobbyAsync(name, max_players, options);
            Debug.Log("Created Lobby " + _connectedLobby.Name + " with " + _connectedLobby.MaxPlayers + " max players!");
            Debug.Log("Lobby Code is " + _connectedLobby.LobbyCode);
            return string.Empty;
        }
        catch (LobbyServiceException l)
        {
            Debug.LogError(l);
            return l.Message;
        }
    }
    
    private async void GetLobbies()
    {
        try
        {
            QueryResponse lobbies = await LobbyService.Instance.QueryLobbiesAsync();
            foreach (Lobby lobby in lobbies.Results)
            {
                Debug.Log(lobby.Id + " " + lobby.Name + " " + lobby.MaxPlayers);
            }
        }
        catch (LobbyServiceException l)
        {
            Debug.LogError(l);
            throw;
        }
    }

    private async void KeepLobbyAlive()
    {
        if (_connectedLobby != null && IsHost())
        {
            _heartbeatElapsedTime += Time.deltaTime;
            if (_heartbeatElapsedTime >= _heartbeatInterval)
            {
                _heartbeatElapsedTime = 0.0f;
                await LobbyService.Instance.SendHeartbeatPingAsync(_connectedLobby.Id);
            }
        }
    }

    private async void UpdateLobbyData()
    {
        if (_connectedLobby != null)
        {
            _lobbyUpdateElapsedTime += Time.deltaTime;
            if (_lobbyUpdateElapsedTime >= _lobbyUpdateInterval)
            {
                _lobbyUpdateElapsedTime = 0.0f;
                _connectedLobby = await LobbyService.Instance.GetLobbyAsync(_connectedLobby.Id);
                ManagerSystems.Instance.GetMenuManager().GetLobbyUpdaterUI().UpdateLobbydata(_connectedLobby);
            }
        }
    }

    public async Task<string> JoinLobbyWithCode(string code)
    {
        try
        {
            Lobby joined_lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(code);
            if (joined_lobby != null)
            {
                _connectedLobby = joined_lobby;
                Debug.Log("Joined lobby: " + _connectedLobby.Name);
            }
            
            return string.Empty;
        }
        catch (LobbyServiceException l)
        {
            Debug.LogError(l);
            return l.Message;
            throw;
        }
    }

    public Lobby GetActiveLobby()
    {
        return _connectedLobby;
    }

    public bool IsHost()
    {
        return _connectedLobby != null && _connectedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }

    public async void RemovePlayerFromConnectedLobby()
    {
        Debug.Log("Disconnecting Player...");
        await LobbyService.Instance.RemovePlayerAsync(_connectedLobby.Id, AuthenticationService.Instance.PlayerId);
        _connectedLobby = null;
    }

    private void OnDestroy()
    {
        // TODO Doesnt work properly
        RemovePlayerFromConnectedLobby();
    }
}
