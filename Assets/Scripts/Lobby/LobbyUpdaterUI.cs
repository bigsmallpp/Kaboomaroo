using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyUpdaterUI : MonoBehaviour
{
    [SerializeField] private GameObject _lobbyPlayerEntryPrefab;
    [SerializeField] private Dictionary<string, GameObject> _lobbyPlayers;
    [SerializeField] private GameObject _lobbyGridView;
    [SerializeField] private TextMeshProUGUI _lobbyCode;

    private const int LOBBY_CODE_LENGTH = 6;

    private void Start()
    {
        _lobbyPlayers = new Dictionary<string, GameObject>();
    }

    public void UpdateLobbydata(Lobby lobby)
    {
        UpdatePlayersInGridView(lobby.Players);
        
        // TODO Set LobbyName
        // LobbyName = active_lobby.Name;
        
        // TODO Set Player Count
        // player_count = active_lobby.Players.Count + "/" + active_lobby.MaxPlayers +" " + Players;
        
        SetLobbyCode(lobby.LobbyCode);
    }
    
    private void UpdatePlayersInGridView(List<Player> players)
    {
        bool entries_changed = false;
        foreach (Player player in players)
        {
            if (!_lobbyPlayers.ContainsKey(player.Id) && player.Data != null && player.Data.ContainsKey("Name"))
            {
                entries_changed = true;
                GameObject lobby_entry = Instantiate(_lobbyPlayerEntryPrefab, _lobbyGridView.transform);
                
                string name_highlight = player.Id == AuthenticationService.Instance.PlayerId ? " (You)" : String.Empty;
                TextMeshProUGUI[] texts = lobby_entry.GetComponentsInChildren<TextMeshProUGUI>();
                texts[1].text = player.Data["Name"].Value + name_highlight;
                
                _lobbyPlayers.Add(player.Id, lobby_entry);
                // TODO Color selection
            }
        }
        
        // Remove players that left
        if (_lobbyPlayers.Count > players.Count)
        {
            entries_changed = true;
            List<string> keys_to_remove = new List<string>();
            
            foreach (KeyValuePair<string, GameObject> lobby_entry in _lobbyPlayers)
            {
                keys_to_remove.Add(lobby_entry.Key);
            }
            
            foreach (Player player in players)
            {
                keys_to_remove.Remove(player.Id);
            }
            
            foreach (string key in keys_to_remove)
            {
                Destroy(_lobbyPlayers[key]);
                _lobbyPlayers.Remove(key);
            }
        }

        if (entries_changed)
        {
            UpdatePlayerIndices(players);
        }
    }

    private void UpdatePlayerIndices(List<Player> players)
    {
        int index = 1;

        Debug.Log(_lobbyPlayers.Count);
        foreach (Player player in players)
        {
            _lobbyPlayers[player.Id].GetComponentInChildren<TextMeshProUGUI>().text = "P" + index.ToString();
            index++;
        }
    }

    private void SetLobbyCode(string code)
    {
        string current_code = _lobbyCode.text.Substring(_lobbyCode.text.Length - 6);
        Debug.Log("Current Code = " + current_code);
        
        if (!current_code.Equals(code))
        {
            _lobbyCode.text = "Lobby Code: " + code;
        }
    }

    public void StartGame()
    {
        ManagerSystems.Instance.GetNetworkingManager().GetLobbyManager().StartGame();
    }
}
