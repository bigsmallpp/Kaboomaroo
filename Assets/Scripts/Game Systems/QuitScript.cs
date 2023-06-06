using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class QuitScript : NetworkBehaviour
{

    // Reference to the LobbyManager singleton
    private LobbyManager lobbyManager;

    private NetworkClient networkClient;

    private void Start()
    {
        // Get references to the singleton instances
        lobbyManager = LobbyManager.Instance;
    }

    public void PerformButtonAction()
    {


        // Remove the player from the lobby
        if (lobbyManager != null)
        {
            lobbyManager.RemovePlayerFromConnectedLobby();
        }

        // Load the LobbyScene
        SceneManager.LoadScene("SampleScene");

        NetworkManager.Singleton.Shutdown();
        Debug.Log("Player Disconnected!");

    }
}

