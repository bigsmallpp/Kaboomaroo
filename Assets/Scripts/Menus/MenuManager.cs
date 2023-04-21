using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private IPManager _ipManager;
    [SerializeField] private MainMenuButtons _menuButtons;
    [SerializeField] private LobbyUpdaterUI _lobbyUpdaterUI;
    
    [SerializeField] private TextMeshProUGUI _ipText;

    public const string IP = "IP: ";
    
    // Start is called before the first frame update
    void Start()
    {
        if (ManagerSystems.Instance != null)
        {
            LobbyManager.Instance.SetJoinedGame(false);
            ManagerSystems.Instance.SetMenuManager(this);
        }
        
        StartCoroutine(WaitForInitAndSetIPAddress());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator WaitForInitAndSetIPAddress()
    {
        while (_ipManager.GetIPStatus() != IPManager.IP_STATUS.SUCCESS)
        {
            yield return new WaitForSeconds(0.5f);
        }
        
        _ipText.text = IP + _ipManager.GetIPAddress();
    }

    public void SetupLobbyScreen()
    {
        _menuButtons.SwitchToLobbyScreenMenu(true);
    }

    public MainMenuButtons GetMainMenuButtons()
    {
        return _menuButtons;
    }

    public LobbyUpdaterUI GetLobbyUpdaterUI()
    {
        return _lobbyUpdaterUI;
    }
}
