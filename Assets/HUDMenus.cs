using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HUDMenus : MonoBehaviour
{

    [Header("Variables/Texts")]
    [SerializeField] private TextMeshProUGUI _textConnectedPlayers;
    [SerializeField] private TextMeshProUGUI _text_Win_Info;


    public void InitializeConnectedPlayers(PlayerSpawner spawner)
    {
        UpdateConnectedPlayerCount(spawner.GetConnectedPlayerCount());
    }

    public void UpdateConnectedPlayerCount(DoubleInt values)
    {
        _textConnectedPlayers.text = "Players alive: " + values._first.ToString();
    }

}
