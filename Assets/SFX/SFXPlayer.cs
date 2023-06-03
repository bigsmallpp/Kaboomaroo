using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXPlayer : MonoBehaviour
{
    public AudioSource bomb;
    public AudioSource game_start;
    public AudioSource button_click;
    public AudioSource you_won;
    public AudioSource step;
    public AudioSource you_died;

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

    public void YouWon()
    {
        you_won.Play();
    }

    public void YouDied()
    {
        you_died.Play();
    }

    public void Step()
    {
        step.Play();
    }

    public void StopStep()
    {
        step.Stop();
    }
}
