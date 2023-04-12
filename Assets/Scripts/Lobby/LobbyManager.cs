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
    private async void Start()
    {
        DontDestroyOnLoad(this);
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += SignedIn;
        AuthenticationService.Instance.SignInFailed += SignInFailed;
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    private void SignedIn()
    {
        Debug.Log("Signed In Player " + AuthenticationService.Instance.PlayerId);
    }

    private void SignInFailed(RequestFailedException e)
    {
        Debug.LogError("e");
    }
    
    private async void CreateLobby()
    {
        try
        {
            // TODO Read Docs
            CreateLobbyOptions options = new CreateLobbyOptions();
        
            string lobby_name = "My Cool Lobby";
            int max_players = 4;
            _connectedLobby = await LobbyService.Instance.CreateLobbyAsync(lobby_name, max_players, options);
            StartCoroutine(KeepLobbyAlive());
        }
        catch (LobbyServiceException l)
        {
            Debug.LogError(l);
            throw;
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

    IEnumerator KeepLobbyAlive()
    {
        float interval = 20.0f;
        float elapsed_time = 0.0f;

        while (_connectedLobby != null)
        {
            elapsed_time += Time.deltaTime;
            if (elapsed_time >= interval)
            {
                elapsed_time = 0.0f;
                Task task = LobbyService.Instance.SendHeartbeatPingAsync(_connectedLobby.Id);
                yield return new WaitUntil(() => task.IsCompleted);
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
            
            return "";
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
}
