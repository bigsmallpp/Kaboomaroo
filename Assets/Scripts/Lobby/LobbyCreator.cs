using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyCreator : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _playerCount;
    [SerializeField] private TMP_InputField _lobbyName;

    public async void CreateLobby()
    {
        string name = _lobbyName.text;
        if (string.IsNullOrEmpty(name))
        {
            return;
        }
        
        int max_players = int.Parse(_playerCount.text);
        string response = await ManagerSystems.Instance.GetNetworkingManager().GetLobbyManager().CreateLobby(name, max_players);

        if (response.Equals(string.Empty))
        {
            ManagerSystems.Instance.GetMenuManager().SetupLobbyScreen();
        }
        else
        {
            // Print Error Text On Screen
        }
    }
}
