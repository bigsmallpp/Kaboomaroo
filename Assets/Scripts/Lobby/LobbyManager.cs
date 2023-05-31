using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class LobbyManager : MonoBehaviour
{
    private static LobbyManager _instance;
    public static LobbyManager Instance => _instance;
    
    private Lobby _connectedLobby;
    
    [Header("Heartbeat Variables")]
    [SerializeField] private float _heartbeatElapsedTime = 0.0f;
    [SerializeField] private float _heartbeatInterval = 20.0f;
    
    [Header("Lobby Update Variables")]
    [SerializeField] private float _lobbyUpdateElapsedTime = 0.0f;
    [SerializeField] private float _lobbyUpdateInterval = 2.5f;

    [Header("Timeout Variables")]
    [SerializeField] private float _allowedIdleTime;
    private bool _applicationInFocusAndNotPaused = true;
    
    private const string KEY_LAST_SEEN = "LastSeen";
    private const string KEY_NAME = "Name";
    private const string KEY_RELAY_CODE = "RelayCode";

    private bool _joinedGame = false;
    
    private List<string> _playerNames = new List<string>()
    {
        "Pristine Platypus",
        "Graceful Gorilla",
        "Happy Hippo",
        "Dreadful Doggo",
        "Amazing Ape",
        "Hectic Harald",
        "Risky Ralf",
        "Revengeful Ronald"
    };
    private async void Start()
    {
        if (_instance)
        {
            _instance._joinedGame = false;
            Destroy(this);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(this);
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += SignedIn;
        AuthenticationService.Instance.SignInFailed += SignInFailed;
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    private async void CreateRelayAndStartHost()
    {
        try
        {
            // Host is not included here
            Allocation relay_alloc = await RelayService.Instance.CreateAllocationAsync(_connectedLobby.MaxPlayers - 1);
            string relay_code = await RelayService.Instance.GetJoinCodeAsync(relay_alloc.AllocationId);
            Debug.Log(relay_code);

            RelayServerData server_data = new RelayServerData(relay_alloc, "dtls");
            
            // Set the Relay Code in the Lobby Variables
            UpdateLobbyOptions options = new UpdateLobbyOptions();
            options.Data = new Dictionary<string, DataObject>()
            {
                {
                    KEY_RELAY_CODE,
                    new DataObject(
                        visibility: DataObject.VisibilityOptions.Member, 
                        value: relay_code)
                }
            };

            _connectedLobby = await LobbyService.Instance.UpdateLobbyAsync(_connectedLobby.Id, options);
            _joinedGame = true;
            
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(server_data);
            NetworkManager.Singleton.StartHost();
            NetworkManager.Singleton.SceneManager.LoadScene("GameplayScene", LoadSceneMode.Single);
        }
        catch (RelayServiceException r)
        {
            // TODO Handle Relay Service Exception
            Debug.LogError(r);
        }
        catch (LobbyServiceException l)
        {
            HandleLobbyServiceException(l);
        }
    }

    private void Update()
    {
        KeepLobbyAlive();
        UpdateLobbyData();
    }

    private void SignedIn()
    {
        Debug.Log("Signed In Player " + AuthenticationService.Instance.PlayerId);
    }

    private void SignInFailed(RequestFailedException e)
    {
        Debug.LogError("e");
    }
    
    public async Task<string> CreateLobby(string name, int max_players)
    {
        try
        {
            // TODO Read Docs
            CreateLobbyOptions options = new CreateLobbyOptions();
            options.Data = new Dictionary<string, DataObject>()
            {
                {
                    KEY_RELAY_CODE,
                    new DataObject(
                        visibility: DataObject.VisibilityOptions.Member, 
                        value: String.Empty)
                }
            };
            
            _connectedLobby = await LobbyService.Instance.CreateLobbyAsync(name, max_players, options);
            Debug.Log("Created Lobby " + _connectedLobby.Name + " with " + _connectedLobby.MaxPlayers + " max players!");
            Debug.Log("Lobby Code is " + _connectedLobby.LobbyCode);
            InitializePlayerDataInfrictionsAndName();
            return string.Empty;
        }
        catch (LobbyServiceException l)
        {
            Debug.LogWarning(l);
            return l.Message;
        }
    }
    
    private async void GetLobbies()
    {
        try
        {
            QueryResponse lobbies = await LobbyService.Instance.QueryLobbiesAsync();
            foreach (Lobby lobby in lobbies.Results)
            {
                Debug.Log(lobby.Id + " " + lobby.Name + " " + lobby.MaxPlayers);
            }
        }
        catch (LobbyServiceException l)
        {
            HandleLobbyServiceException(l);
        }
    }

    private async void KeepLobbyAlive()
    {
        if (_connectedLobby != null && IsHost())
        {
            _heartbeatElapsedTime += Time.deltaTime;
            if (_heartbeatElapsedTime >= _heartbeatInterval)
            {
                _heartbeatElapsedTime = 0.0f;
                await LobbyService.Instance.SendHeartbeatPingAsync(_connectedLobby.Id);
            }
        }
    }

    private async void UpdateLobbyData()
    {
        try
        {
            if (_connectedLobby != null)
            {
                _lobbyUpdateElapsedTime += Time.deltaTime;
                if (_lobbyUpdateElapsedTime >= _lobbyUpdateInterval)
                {
                    _lobbyUpdateElapsedTime = 0.0f;
                    _connectedLobby = await LobbyService.Instance.GetLobbyAsync(_connectedLobby.Id);
                    
                    if (!_joinedGame)
                    {
                        Debug.Log("Update Lobby UI called");
                        ManagerSystems.Instance.GetMenuManager().GetLobbyUpdaterUI().UpdateLobbydata(_connectedLobby, IsHost());
                    }
                    
                    SendPlayerheartbeat();
                    CheckRelayCodeSet();
                }
            }
        }
        catch (LobbyServiceException l)
        {
            HandleLobbyServiceException(l);
        }
    }

    public async Task<string> JoinLobbyWithCode(string code)
    {
        try
        {
            Lobby joined_lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(code);
            if (joined_lobby != null)
            {
                _connectedLobby = joined_lobby;
                Debug.Log("Joined lobby: " + _connectedLobby.Name);
                
                InitializePlayerDataInfrictionsAndName();
            }
            
            return string.Empty;
        }
        catch (LobbyServiceException l)
        {
            Debug.LogWarning(l);
            return l.Message;
        }
    }

    public Lobby GetActiveLobby()
    {
        return _connectedLobby;
    }

    public bool IsHost()
    {
        return _connectedLobby != null && _connectedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }

    public async void RemovePlayerFromConnectedLobby(string ID = "")
    {
        Debug.Log("Disconnecting Player...");
        try
        {
            string id_to_remove = ID == "" ? AuthenticationService.Instance.PlayerId : ID;
            await LobbyService.Instance.RemovePlayerAsync(_connectedLobby.Id, id_to_remove);
            _connectedLobby = null;
        }
        catch (LobbyServiceException l)
        {
            HandleLobbyServiceException(l);
        }
    }

    private void OnDestroy()
    {
        // TODO Doesnt work properly on force quit
        if (_instance == this)
        {
            RemovePlayerFromConnectedLobby();
        }
    }

    public void SendPlayerheartbeat()
    {
        if (_connectedLobby == null)
        {
            return;
        }
        
        // Client and Host reset their last active time
        HandleIdleInfrictionsClient();
        
        // Host checks inactive times and kicks after threshold
        if (IsHost())
        {
            HostCheckInactivePlayers();
        }
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (Application.platform != RuntimePlatform.Android)
            return;
        
        Debug.Log("Focus of App changed to " + hasFocus);
        _applicationInFocusAndNotPaused = hasFocus;
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (Application.platform != RuntimePlatform.Android)
            return;
        
        Debug.Log("Pause Status of App changed to " + pauseStatus);
        _applicationInFocusAndNotPaused = !pauseStatus;
    }

    private async void InitializePlayerDataInfrictionsAndName()
    {
        try
        {
            UpdatePlayerOptions options = new UpdatePlayerOptions();
            options.Data = new Dictionary<string, PlayerDataObject>()
            {
                {
                    KEY_LAST_SEEN, new PlayerDataObject(
                        visibility: PlayerDataObject.VisibilityOptions.Member,
                        value: DateTime.Now.ToString())
                },
                {
                    KEY_NAME, new PlayerDataObject(
                        visibility: PlayerDataObject.VisibilityOptions.Member,
                        value: PickUniquePlayerName())
                }
            };

            _connectedLobby = await LobbyService.Instance.UpdatePlayerAsync(_connectedLobby.Id, AuthenticationService.Instance.PlayerId, options);
        }
        catch (LobbyServiceException l)
        {
            HandleLobbyServiceException(l);
        }
    }

    private async void HandleIdleInfrictionsClient()
    {
        try
        {
            if (_applicationInFocusAndNotPaused)
            {
                UpdatePlayerOptions options = new UpdatePlayerOptions();
                options.Data = new Dictionary<string, PlayerDataObject>()
                {
                    {
                        KEY_LAST_SEEN, new PlayerDataObject(
                            visibility: PlayerDataObject.VisibilityOptions.Member,
                            value: DateTime.Now.ToString())
                    }
                };
                _connectedLobby = await LobbyService.Instance.UpdatePlayerAsync(_connectedLobby.Id, AuthenticationService.Instance.PlayerId, options);
            }
        }
        catch (LobbyServiceException l)
        {
            HandleLobbyServiceException(l);
        }
    }

    private async void HostCheckInactivePlayers()
    {
        try
        {
            foreach (Player player in _connectedLobby.Players)
            {
                if (player.Data != null && player.Data.ContainsKey(KEY_LAST_SEEN))
                {
                    DateTime last_seen = DateTime.Parse(player.Data[KEY_LAST_SEEN].Value);
                    float time_diff = (DateTime.Now - last_seen).Seconds;
                    
                    Debug.Log("(" + player.Id + ") Time since no response: " + time_diff);
                    // Kick player if he didn't respond after _allowedIdleTime Seconds
                    if (time_diff >= _allowedIdleTime)
                    {
                        await LobbyService.Instance.RemovePlayerAsync(_connectedLobby.Id, player.Id);
                    }
                }
            }
        }
        catch (LobbyServiceException l)
        {
            HandleLobbyServiceException(l);
        }
    }

    private void HandleLobbyServiceException(LobbyServiceException l)
    {
        // You were kicked from the Lobby
        if (l.Reason == LobbyExceptionReason.PlayerNotFound)
        {
            Debug.Log("You were kicked from the session!");
            _connectedLobby = null;
            ManagerSystems.Instance.GetMenuManager().GetMainMenuButtons().SwitchToMainMenu();
        }
        else if (l.Reason == LobbyExceptionReason.LobbyNotFound)
        {
            Debug.Log("The lobby you tried to access, doesn't exist anymore!");
            _connectedLobby = null;
            ManagerSystems.Instance.GetMenuManager().GetMainMenuButtons().SwitchToMainMenu();
        }
        else
        {
            Debug.LogError("Unhandled Exception Case!");
            Debug.LogError(l);
        }
    }

    private string PickUniquePlayerName()
    {
        while (true)
        {
            int random_index = Random.Range(0, _playerNames.Count - 1);
            bool unique = true;
            string temp = _playerNames[random_index];

            foreach (Player player in _connectedLobby.Players)
            {
                if (player.Data != null && player.Data.ContainsKey(KEY_NAME) && player.Data[KEY_NAME].Value.Equals(temp))
                {
                    unique = false;
                    break;
                }
            }

            if (unique)
            {
                return temp;
            }
        }
    }

    public void StartGame()
    {
        if (_connectedLobby == null)
        {
            return;
        }

        if (IsHost())
        {
            CreateRelayAndStartHost();
        }
        else
        {
            JoinRelayAndStartClient();
        }
    }

    private void CheckRelayCodeSet()
    {
        if (_joinedGame)
        {
            return;
        }
        
        if (IsHost())
        {
            return;
        }

        string relay_code = _connectedLobby.Data[KEY_RELAY_CODE].Value;
        if (_connectedLobby.Data != null && !relay_code.Equals(string.Empty))
        {
            StartGame();
        }
    }

    private async void JoinRelayAndStartClient()
    {
        try
        {
            string relay_code = _connectedLobby.Data[KEY_RELAY_CODE].Value;
            
            // Host is not included here
            JoinAllocation relay_alloc = await RelayService.Instance.JoinAllocationAsync(relay_code);
            RelayServerData server_data = new RelayServerData(relay_alloc, "dtls");

            _joinedGame = true;
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(server_data);
            NetworkManager.Singleton.StartClient();
        }
        catch (RelayServiceException r)
        {
            Debug.LogError(r);
        }
    }

    public void SetJoinedGame(bool val)
    {
        _joinedGame = val;
    }

    public int GetConnectedPlayerCount()
    {
        return _connectedLobby == null ? 0 : _connectedLobby.Players.Count;
    }

    public async void ResetRelayCode()
    {
        UpdateLobbyOptions options = new UpdateLobbyOptions();
        options.Data = new Dictionary<string, DataObject>()
        {
            {
                KEY_RELAY_CODE,
                new DataObject(
                    visibility: DataObject.VisibilityOptions.Member, 
                    value: string.Empty)
            }
        };

        _connectedLobby = await LobbyService.Instance.UpdateLobbyAsync(_connectedLobby.Id, options);
    }

    public void InvalidateLobby()
    {
        if (_connectedLobby == null)
        {
            return;
        }

        _connectedLobby = null;
        SetJoinedGame(false);
    }
}
