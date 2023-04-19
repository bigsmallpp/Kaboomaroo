using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class GameMenuManager : MonoBehaviour
{
    [Header("Menus")]
    [SerializeField] private GameObject _menuPreGame;

    [Header("Variables/Texts")] 
    [SerializeField] private TextMeshProUGUI _textConnectedPlayers;

    public void HidePreGameMenuAndSwitchToInGameMenu()
    {
        _menuPreGame.SetActive(false);
    }

    private void SwitchToCountdown()
    {
        _textConnectedPlayers.text = "3.0";
    }

    public void UpdateConnectedPlayerCount(DoubleInt values)
    {
        _textConnectedPlayers.text = values._first + "/" + values._second;
    }

    public void InitializePreGameEvents(PlayerSpawner spawner)
    {
        spawner.onPlayerConnected.AddListener(UpdateConnectedPlayerCount);
        spawner.onAllPlayersConnected.AddListener(SwitchToCountdown);
        spawner.onCountdownTickDown.AddListener(UpdateCountdown);
        spawner.onCountdownOver.AddListener(HidePreGameMenuAndSwitchToInGameMenu);
    }

    public void InitializeConnectedPlayers(PlayerSpawner spawner)
    {
        UpdateConnectedPlayerCount(spawner.GetConnectedPlayerCount());
    }

    private void UpdateCountdown(float new_val)
    {
        _textConnectedPlayers.text = new_val.ToString("0.0");
    }
}
