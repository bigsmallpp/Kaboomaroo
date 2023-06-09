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
    public AudioSource place_bomb;
    public AudioSource collect_item;

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

    public void PlaceBomb()
    {
        place_bomb.Play();
    }

    public void CollectItem()
    {
        collect_item.Play();
    }
}
