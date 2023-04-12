using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JoinLobbyLogic : MonoBehaviour
{
    [SerializeField] private TMP_InputField _lobbyCode;
    [SerializeField] private LobbyManager _lobbyManager;
    [SerializeField] private MenuManager _menuManager;
    [SerializeField] private Button _joinLobbyButton;
    [SerializeField] private TextMeshProUGUI _errorText;

    public const int LOBBY_CODE_LENGTH = 6;
    public async void InvokeJoinLobby()
    {
        _joinLobbyButton.enabled = false;
        _errorText.text = "";
        string code = _lobbyCode.text;
        if (string.IsNullOrEmpty(code) || code.Length < LOBBY_CODE_LENGTH)
        {
            _joinLobbyButton.enabled = true;
            _errorText.text = "Invalid Lobby Code entered!";
            return;
        }
        
        string join = await _lobbyManager.JoinLobbyWithCode(code);
        
        // Joined Successfully, no error
        if (join.Equals(""))
        {
            _menuManager.SetupLobbyScreen();
        }
        // String contains Error Text
        else
        {
            _errorText.text = join + "!";
        }
        
        _joinLobbyButton.enabled = true;
    }
}
