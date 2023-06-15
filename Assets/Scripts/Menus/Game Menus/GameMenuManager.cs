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
    [SerializeField] private GameObject _menuInGame;

    [Header("Variables/Texts")] 
    [SerializeField] private TextMeshProUGUI _textConnectedPlayers;
    [SerializeField] private TextMeshProUGUI _textYouDied;
    [SerializeField] private TextMeshProUGUI _textYouWon;

    [Header("Unity Events")]
    public UnityEvent onYouDied;

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
        spawner.onCountdownOver.AddListener(SwitchToIngameMenu);
    }

    public void InitializeConnectedPlayers(PlayerSpawner spawner)
    {
        UpdateConnectedPlayerCount(spawner.GetConnectedPlayerCount());
    }

    private void UpdateCountdown(float new_val)
    {
        _textConnectedPlayers.text = new_val.ToString("0.0");
    }

    public void InitializeInGameEvents(NetworkedGameMenus menus)
    {
        menus.onShowDeathScreen.AddListener(SetDeathMessageActive);
        menus.onShowWinnerScreen.AddListener(SetWinMessageActive);
    }

    public void SwitchToIngameMenu()
    {
        _menuPreGame.gameObject.SetActive(false);
        _menuInGame.gameObject.SetActive(true);
    }
    
    public void SwitchPregameMenu()
    {
        _menuInGame.gameObject.SetActive(false);
        _menuPreGame.gameObject.SetActive(true);
    }

    public void SetDeathMessageActive(bool val)
    {
        _textYouDied.gameObject.SetActive(val);
        onYouDied.Invoke();
        StartCoroutine(AnimatePostGameMessage(_textYouDied.gameObject));
    }

    public void SetWinMessageActive(bool val)
    {
        _textYouWon.gameObject.SetActive(val);
        StartCoroutine(AnimatePostGameMessage(_textYouWon.gameObject));
    }

    IEnumerator AnimatePostGameMessage(GameObject text)
    {
        float target_size = text.GetComponent<TextMeshProUGUI>().fontSize;
        text.GetComponent<TextMeshProUGUI>().fontSize = 0.0f;
        float end = 1.0f;
        float current_time = 0.0f;

        while (current_time <= end)
        {
            float new_size = Mathf.Lerp(text.GetComponent<TextMeshProUGUI>().fontSize, target_size, current_time);
            text.GetComponent<TextMeshProUGUI>().fontSize = new_size;
            current_time += Time.deltaTime;
            yield return null;
        }
    }
}
