using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using Unity.Services.Authentication;

public class MainMenuButtons : MonoBehaviour
{
    [Header("All the available Menus")]
    [SerializeField] private GameObject _mainMenu;
    [SerializeField] private GameObject _joinLobbyMenu;
    [SerializeField] private GameObject _createLobbyMenu;
    [SerializeField] private GameObject _optionsMenu;
    [SerializeField] private GameObject _lobbyScreen;
    [SerializeField] private GameObject _activeMenu;
    [SerializeField] private GameObject _title;
    [SerializeField] private GameObject _credits;

    [Header("The connecting screen")]
    [SerializeField] private GameObject _connectingToLobbyScreen;
    
    [Header("The Error screen")]
    [SerializeField] private GameObject _errorScreen;
    [SerializeField] private Button _errorRetry;
    
    [Header("The Status")]
    [SerializeField] private TextMeshProUGUI _connectionStatus;
    
    private CustomInput _input;
    public bool goBackToFirstScene = false;
    
    public enum CONNECTION_STATUS
    {
        CONNECTED,
        RETRYING,
        DISCONNECTED
    }

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
        _title.SetActive(true);
        _activeMenu = _mainMenu;
    }
    
    public void SwitchToOptionsMenu()
    {
        _activeMenu.SetActive(false);
        _title.SetActive(false);
        _optionsMenu.SetActive(true);
        _activeMenu = _optionsMenu;
    }

    public void SwitchToCreditsMenu()
    {
        _activeMenu.SetActive(false);
        _title.SetActive(false);
        _credits.SetActive(true);
        _activeMenu = _credits;
    }

    public void SwitchToJoinLobbyMenu()
    {
        _activeMenu.SetActive(false);
        _title.SetActive(false);
        _joinLobbyMenu.SetActive(true);
        _activeMenu = _joinLobbyMenu;
    }
    
    public void SwitchToCreateLobbyMenu()
    {
        _activeMenu.SetActive(false);
        _title.SetActive(false);
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
        _title.SetActive(false);
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

    public void SetAndShowErrorMessage(string error)
    {
        _errorScreen.SetActive(true);
        _errorScreen.GetComponentInChildren<TextMeshProUGUI>().text = error;
    }

    public void HideAndResetErrorMessage()
    {
        _errorScreen.SetActive(false);
        _errorScreen.GetComponentInChildren<TextMeshProUGUI>().text = String.Empty;
    }

    public void SetStatusText(CONNECTION_STATUS current_state)
    {
        switch (current_state)
        {
            case CONNECTION_STATUS.CONNECTED:
                _connectionStatus.text = "Connected";
                _connectionStatus.color = Color.green;
                break;
            
            case CONNECTION_STATUS.RETRYING:
                _connectionStatus.text = "Retrying...";
                _connectionStatus.color = Color.yellow;
                break;
            
            case CONNECTION_STATUS.DISCONNECTED:
                _connectionStatus.text = "Disconnected";
                _connectionStatus.color = Color.red;
                break;
            
            default:
                break;
        }
    }

    public void RetryConnection()
    {
        _errorRetry.interactable = false;
        LobbyManager.Instance.RetryAuthLogin("No Internet Connection");
        StartCoroutine(DelayRetryButton());
    }

    IEnumerator DelayRetryButton()
    {
        yield return new WaitForSecondsRealtime(5);
        _errorRetry.interactable = true;
    }

    public void openCreditsUrl()
    {
        string _url = "https://rentry.co/ee6zu/";
        Application.OpenURL(_url);
    }
}
