using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Initializer : MonoBehaviour
{
    [SerializeField] private GameMenuManager _gameMenuManager;
    [SerializeField] private PlayerSpawner _playerSpawner;

    [Header("Prefabs")]
    [SerializeField] private GameObject _prefPlayerSpawner;
    private void Start()
    {
        if (NetworkManager.Singleton.IsServer == false)
        {
            _playerSpawner = GameObject.FindWithTag("PlayerSpawner").GetComponent<PlayerSpawner>();
            _gameMenuManager.GetComponent<GameMenuManager>().InitializePreGameEvents(_playerSpawner);
            _gameMenuManager.InitializeConnectedPlayers(_playerSpawner);
        }
        else
        {
            GameObject player_spawner = Instantiate(_prefPlayerSpawner);

            _playerSpawner = player_spawner.GetComponent<PlayerSpawner>();
            _gameMenuManager.GetComponent<GameMenuManager>().InitializePreGameEvents(_playerSpawner);
            player_spawner.GetComponent<NetworkObject>().Spawn();
        }
    }
}
