using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkingManager : MonoBehaviour
{
    [SerializeField] private LobbyManager _lobbyManager;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public LobbyManager GetLobbyManager()
    {
        return _lobbyManager;
    }
}
