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
        
        // TODO Set Lobby Code
        // LobbyCode = active_lobby.LobbyCode;
    }
    
    private void UpdatePlayersInGridView(List<Player> players)
    {
        bool entries_changed = false;
        foreach (Player player in players)
        {
            if (!_lobbyPlayers.ContainsKey(player.Id))
            {
                entries_changed = true;
                GameObject lobby_entry = Instantiate(_lobbyPlayerEntryPrefab, _lobbyGridView.transform);
                
                string name_highlight = player.Id == AuthenticationService.Instance.PlayerId ? " (You)" : String.Empty;
                TextMeshProUGUI[] texts = lobby_entry.GetComponentsInChildren<TextMeshProUGUI>();
                texts[1].text = "Random text idk" + name_highlight;
                
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
            UpdatePlayerIndices();
        }
    }

    private void UpdatePlayerIndices()
    {
        int index = 1;
        foreach (KeyValuePair<string, GameObject> lobby_entry in _lobbyPlayers)
        {
            // Should return first TMP in Children
            lobby_entry.Value.GetComponentInChildren<TextMeshProUGUI>().text = "P" + index.ToString();
            index++;
        }
    }
}
