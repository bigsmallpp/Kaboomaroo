using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class QuitScript : MonoBehaviour
{
    public void PerformButtonAction()
    {
        LobbyManager lobbyManager = LobbyManager.Instance;
        // Remove the player from the lobby
        if (lobbyManager != null)
        {
            lobbyManager.RemovePlayerFromConnectedLobby();
        }

        // Load the LobbyScene
        SceneManager.LoadScene("MainMenuDisconnect");

        NetworkManager.Singleton.Shutdown();
        Debug.Log("Player Disconnected!");

    }
}

