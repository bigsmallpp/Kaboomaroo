using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXPlayer : MonoBehaviour
{
    public AudioSource bomb;
    public AudioSource game_start;
    public AudioSource button_click;

    public void PlayBomb()
    {
        bomb.Play();
    }

    public void PlayGameStart()
    {
        game_start.Play();
    }

    public void Click()
    {
        button_click.Play();
    }
}
