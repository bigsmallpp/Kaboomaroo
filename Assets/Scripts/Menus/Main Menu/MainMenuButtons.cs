using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuButtons : MonoBehaviour
{
    [Header("All the available Menus")]
    [SerializeField] private GameObject _mainMenu;
    [SerializeField] private GameObject _joinLobbyMenu;
    [SerializeField] private GameObject _createLobbyMenu;
    [SerializeField] private GameObject _optionsMenu;
    [SerializeField] private GameObject _lobbyScreen;
    
    private GameObject _activeMenu;

    private void Start()
    {
        _activeMenu = _mainMenu;
    }

    private void Update()
    {
        HandleBackButtonPress();
    }

    public void SwitchToMainMenu()
    {
        _activeMenu.SetActive(false);
        _mainMenu.SetActive(true);
        _activeMenu = _mainMenu;
    }
    
    public void SwitchToOptionsMenu()
    {
        _activeMenu.SetActive(false);
        _optionsMenu.SetActive(true);
        _activeMenu = _optionsMenu;
    }
    
    public void SwitchToJoinLobbyMenu()
    {
        _activeMenu.SetActive(false);
        _joinLobbyMenu.SetActive(true);
        _activeMenu = _joinLobbyMenu;
    }
    
    public void SwitchToCreateLobbyMenu()
    {
        _activeMenu.SetActive(false);
        _createLobbyMenu.SetActive(true);
        _activeMenu = _createLobbyMenu;
    }
    
    public void SwitchToLobbyScreenMenu()
    {
        _activeMenu.SetActive(false);
        _lobbyScreen.SetActive(true);
        _activeMenu = _lobbyScreen;
    }

    private void HandleBackButtonPress()
    {
        // For the Android App
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                EmulateBackArrow();
            }
        }
        // For local debugging
        else if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                EmulateBackArrow();
            }
        }
    }

    private void EmulateBackArrow()
    {
        if (_activeMenu == _mainMenu)
        {
            Application.Quit();
        }
        else if (_activeMenu == _lobbyScreen)
        {
            // TODO Leave lobby Routine goes here
            SwitchToMainMenu();
        }
        else if (_activeMenu == _optionsMenu || _activeMenu == _createLobbyMenu || _activeMenu == _joinLobbyMenu)
        {
            SwitchToMainMenu();
        }
    }
}
