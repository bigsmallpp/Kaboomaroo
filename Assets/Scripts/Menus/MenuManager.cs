using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private IPManager _networkManager;
    [SerializeField] private MainMenuButtons _menuButtons;
    
    [SerializeField] private TextMeshProUGUI _ipText;

    public const string IP = "IP: ";
    
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
        StartCoroutine(WaitForInitAndSetIPAddress());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator WaitForInitAndSetIPAddress()
    {
        while (_networkManager.GetIPStatus() != IPManager.IP_STATUS.SUCCESS)
        {
            yield return new WaitForSeconds(0.5f);
        }
        
        _ipText.text = IP + _networkManager.GetIPAddress();
    }

    public void SetupLobbyScreen()
    {
        Lobby active_lobby = ManagerSystems.Instance.GetNetworkingManager().GetLobbyManager().GetActiveLobby();

        foreach (Player player in active_lobby.Players)
        {
            // Add player to GridView
        }
        
        // TODO Set LobbyName
        // LobbyName = active_lobby.Name;
        
        // TODO Set Player Count
        // player_count = active_lobby.Players.Count + "/" + active_lobby.MaxPlayers +" " + Players;
        
        // TODO Set Lobby Code
        // LobbyCode = active_lobby.LobbyCode;
        
        _menuButtons.SwitchToLobbyScreenMenu();
    }

    public MainMenuButtons GetMainMenuButtons()
    {
        return _menuButtons;
    }
}
