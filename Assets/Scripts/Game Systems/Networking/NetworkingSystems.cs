using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkingSystems : MonoBehaviour
{
    [SerializeField] private LobbyManager _lobbyManager;
    
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
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
