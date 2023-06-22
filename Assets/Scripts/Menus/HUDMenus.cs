using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HUDMenus : MonoBehaviour
{

    [Header("Variables/Texts")]
    [SerializeField] private TextMeshProUGUI _textConnectedPlayers;
    [SerializeField] private TextMeshProUGUI _text_Win_Info;
    public PlayerSpawner spawner;

    private void Start()
    {
        findPlayerSpawner();
    }

    private void Update()
    {
        if (spawner == null)
        {
            findPlayerSpawner();
        }
        else
        {
            UpdateAlivePlayerCount(spawner.active_player_count.Value);
        } 
    }

    private void findPlayerSpawner()
    {
        spawner = GameObject.FindObjectOfType<PlayerSpawner>();
    }

    public void InitializeConnectedPlayers(PlayerSpawner spawner)
    {
        UpdateConnectedPlayerCount(spawner.GetConnectedPlayerCount());
    }

    public void UpdateConnectedPlayerCount(DoubleInt values)
    {
        _textConnectedPlayers.text = "Players alive: " + values._first.ToString();
    }

    public void UpdateAlivePlayerCount(int players)
    {
        _textConnectedPlayers.text = "Players alive: " + players.ToString();
    }
}
