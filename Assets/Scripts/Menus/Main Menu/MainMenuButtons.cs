using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MainMenuButtons : MonoBehaviour
{
    [Header("All the available Menus")]
    [SerializeField] private GameObject _mainMenu;
    [SerializeField] private GameObject _joinLobbyMenu;
    [SerializeField] private GameObject _createLobbyMenu;
    [SerializeField] private GameObject _optionsMenu;
    [SerializeField] private GameObject _lobbyScreen;
    [SerializeField] private GameObject _activeMenu;
    
    [Header("The connecting screen")]
    [SerializeField] private GameObject _connectingToLobbyScreen;
    
    private CustomInput _input;

    private void Awake()
    {
        _input = new CustomInput();
        _input.CheckForButton.CheckForButtonAction.performed += OnEscapeButtonPressed;
    }

    private void Start()
    {
    }

    private void Update()
    {
        //HandleBackButtonPress();
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
    
    public void SwitchToLobbyScreenMenu(bool connecting_animation)
    {
        if (!connecting_animation)
        {
            SwitchToLobbyScreenInstant();
            return;
        }

        StartCoroutine(PlayConnectingAnimation());
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

    private void SwitchToLobbyScreenInstant()
    {
        _activeMenu.SetActive(false);
        _lobbyScreen.SetActive(true);
        _activeMenu = _lobbyScreen;
    }

    IEnumerator PlayConnectingAnimation()
    {
        _activeMenu.SetActive(false);
        _connectingToLobbyScreen.SetActive(true);
        _lobbyScreen.SetActive(true);
        _activeMenu = _connectingToLobbyScreen;

        while (true)
        {
            if (_lobbyScreen.GetComponent<LobbyUpdaterUI>().GetInitializedPlayersOnLobbyScreen() > 0)
            {
                break;
            }
            yield return new WaitForSeconds(0.3f);
        }
        
        _activeMenu.SetActive(false);
        _activeMenu = _lobbyScreen;
    }

    private void OnEnable()
    {
        _input.Enable();
        _input.CheckForButton.CheckForButtonAction.performed += OnEscapeButtonPressed;
    }

    private void OnDisable()
    {
        _input.Disable();
        _input.CheckForButton.CheckForButtonAction.performed -= OnEscapeButtonPressed;
    }

    private void OnEscapeButtonPressed(InputAction.CallbackContext context) //Template for adding listener on buttons and actions
    {
        if (context.action.triggered && context.action.ReadValue<float>() != 0 && context.action.phase == InputActionPhase.Performed)
        {
            EmulateBackArrow();
        }
    }
}
