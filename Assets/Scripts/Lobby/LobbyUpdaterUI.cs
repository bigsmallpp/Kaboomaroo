using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUpdaterUI : MonoBehaviour
{
    [SerializeField] private GameObject _lobbyPlayerEntryPrefab;
    [SerializeField] private Dictionary<string, GameObject> _lobbyPlayers;
    [SerializeField] private GameObject _lobbyGridView;
    [SerializeField] private TextMeshProUGUI _lobbyCode;
    [SerializeField] private Button _buttonStartGame;
    [SerializeField] private Button _buttonDecreaseRound;
    [SerializeField] private Button _buttonIncreaseRound;
    [SerializeField] private List<Color> _playerColors;
    [SerializeField] private TextMeshProUGUI _lobbyName;
    [SerializeField] private TextMeshProUGUI _playerCount;
    public TMPro.TextMeshProUGUI textField;

    private const int LOBBY_CODE_LENGTH = 6;

    private void Start()
    {
        if(GameSettings.Instance != null)
        {
            updateText();
        }
        _lobbyPlayers = new Dictionary<string, GameObject>();
    }

    public void UpdateLobbydata(Lobby lobby, bool is_host)
    {
        UpdatePlayersInGridView(lobby.Players);
        UpdateStartGameButton(is_host);
        
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
        string host_name = string.Empty;

        Debug.Log(_lobbyPlayers.Count);
        foreach (Player player in players)
        {
            _lobbyPlayers[player.Id].GetComponentInChildren<TextMeshProUGUI>().text = "P" + index.ToString();
            _lobbyPlayers[player.Id].GetComponentInChildren<RawImage>().color = _playerColors[index - 1];

            if(index == 1)
            {
                host_name = _lobbyPlayers[player.Id].GetComponentsInChildren<TextMeshProUGUI>()[1].text;
                _lobbyName.text = host_name.Replace(" (You)", "") + "'s Cool Lobby";
            }

            index++;
        }

        _playerCount.text = _lobbyPlayers.Count.ToString() + "/4";
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
        GameSettings.Instance.setGameMode();
        LobbyManager.Instance.StartGame();
    }

    private void UpdateStartGameButton(bool is_host)
    {
        _buttonStartGame.gameObject.SetActive(is_host);
        _buttonDecreaseRound.gameObject.SetActive(is_host);
        _buttonIncreaseRound.gameObject.SetActive(is_host);
    }

    private void updateText()
    {
        if (GameSettings.Instance.getRoundCount() == 1) textField.SetText("Rounds: 1");
        if (GameSettings.Instance.getRoundCount() == 3) textField.SetText("Rounds: 3");
        if (GameSettings.Instance.getRoundCount() == 5) textField.SetText("Rounds: 5");
    }

    public void increaseRound()
    {
        GameSettings.Instance.increaseRounds();
        updateText();
    }

    public void decreaseRound()
    {
        GameSettings.Instance.decreaseRounds();
        updateText();
    }

    public void DisableStartGameButton()
    {
        _buttonStartGame.interactable = false;
    }

    public int GetInitializedPlayersOnLobbyScreen()
    {
        return _lobbyGridView.transform.Cast<Transform>().ToList().Count;
    }
}
