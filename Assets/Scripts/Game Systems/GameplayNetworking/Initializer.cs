using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Initializer : MonoBehaviour
{
    [SerializeField] private GameMenuManager _gameMenuManager;
    [SerializeField] private PlayerSpawner _playerSpawner;
    [SerializeField] private TileManager _tileManager;

    [Header("Prefabs")]
    [SerializeField] private GameObject _prefPlayerSpawner;
    [SerializeField] private GameObject _prefTileManager;

    [Header("Components in Scene for Initializing")]
    [SerializeField] private Tilemap _tilesDestructible;
    [SerializeField] private Tilemap _tilesIndestructible;
    
    private void Start()
    {
        if (NetworkManager.Singleton.IsServer == false)
        {
            _playerSpawner = GameObject.FindWithTag("PlayerSpawner").GetComponent<PlayerSpawner>();
            _gameMenuManager.GetComponent<GameMenuManager>().InitializePreGameEvents(_playerSpawner);
            _gameMenuManager.InitializeConnectedPlayers(_playerSpawner);
            
            _tileManager = GameObject.FindWithTag("TileManager").GetComponent<TileManager>();
        }
        else
        {
            GameObject player_spawner = Instantiate(_prefPlayerSpawner);

            _playerSpawner = player_spawner.GetComponent<PlayerSpawner>();
            _gameMenuManager.GetComponent<GameMenuManager>().InitializePreGameEvents(_playerSpawner);
            player_spawner.GetComponent<NetworkObject>().Spawn();

            GameObject tile_manager = Instantiate(_prefTileManager);
            _tileManager = tile_manager.GetComponent<TileManager>();
            tile_manager.GetComponent<NetworkObject>().Spawn();
        }
        
        _tileManager.SetTileMaps(_tilesDestructible, _tilesIndestructible);
    }
}
