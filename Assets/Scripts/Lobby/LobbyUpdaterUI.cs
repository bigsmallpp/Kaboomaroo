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
        Debug.Log(players.Count + " Players In Lobby");
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

        if (_lobbyPlayers.Count > players.Count)
        {
            foreach (Player player in players)
            {
                if (_lobbyPlayers.ContainsKey(player.Id))
                {
                    Destroy(_lobbyPlayers[player.Id]);
                    _lobbyPlayers.Remove(player.Id);
                }
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
