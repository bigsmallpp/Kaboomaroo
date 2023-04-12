using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ManagerSystems : MonoBehaviour
{
    private static ManagerSystems _manager;
    public static ManagerSystems Instance => _manager;

    [SerializeField] private MenuManager _menuManager;
    [SerializeField] private NetworkingManager _networkingManager;
    
    // Start is called before the first frame update
    void Start()
    {
        if (_manager == null)
        {
            _manager = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public NetworkingManager GetNetworkingManager()
    {
        return _networkingManager;
    }

    public MenuManager GetMenuManager()
    {
        return _menuManager;
    }
}
