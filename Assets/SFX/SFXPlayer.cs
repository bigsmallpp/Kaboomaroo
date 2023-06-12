using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXPlayer : MonoBehaviour
{
    public AudioSource bomb;
    //public AudioSource game_start;
    public AudioSource button_click;
    public AudioSource you_won;
    public AudioSource step;
    public AudioSource you_died;
    public AudioSource place_bomb;
    public AudioSource collect_item;

    public float _bomb_sound_setting = 2.0f;
    public float _button_click_sound_setting = 2.0f;
    public float _you_won_sound_setting = 2.0f;
    public float _step_sound_setting = 2.0f;
    public float _you_died_sound_setting = 2.0f;
    public float _place_bomb_sound_setting = 2.0f;
    public float _collect_item_sound_setting = 2.0f;

    public float _bg_music_menu_setting = 2.0f;
    public float _bg_music_ingame_setting = 2.0f;
    public float _start_music_setting = 2.0f;

    public AudioSource _bg_music_menu;
    public AudioSource _bg_music_ingame;
    public AudioSource _start_music;

    private static SFXPlayer _player;
    public static SFXPlayer Instance => _player;

    public float sound_volume = 1.0f;
    void Start()
    {
        if (_player == null)
        {
            _player = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
        initSoundSettings();
    }

    private void initSoundSettings()
    {
        Debug.Log("KEK");
        if (_bomb_sound_setting == 2.0f && bomb != null)
        {
            _bomb_sound_setting = bomb.volume;
            Debug.Log("Updated _bomb_sound_setting");
        }
        if (_button_click_sound_setting == 2.0f && button_click != null)
        {
            _button_click_sound_setting = button_click.volume;
            Debug.Log("Updated _button_click_sound_setting");
        }
        if (_you_won_sound_setting == 2.0f && you_won != null)
        {
            _you_won_sound_setting = you_won.volume;
            Debug.Log("Updated _you_won_sound_setting");
        }
        if (_step_sound_setting == 2.0f && step != null)
        {
            _step_sound_setting = step.volume;
            Debug.Log("Updated _step_sound_setting");
        }
        if (_you_died_sound_setting == 2.0f && you_died != null)
        {
            _you_died_sound_setting = you_died.volume;
            Debug.Log("Updated _you_died_sound_setting");
        }
        if (_place_bomb_sound_setting == 2.0f && place_bomb != null)
        {
            _place_bomb_sound_setting = place_bomb.volume;
            Debug.Log("Updated _place_bomb_sound_setting");
        }
        if (_collect_item_sound_setting == 2.0f && collect_item != null)
        {
            _collect_item_sound_setting = collect_item.volume;
            Debug.Log("Updated _collect_item_sound_setting");
        }
        if (_start_music_setting == 2.0f)
        {
            Debug.Log("Updated _start_music_setting called KOK");
            GameObject StartMusic = GameObject.FindGameObjectWithTag("StartMusic");
            if (StartMusic != null)
            {
                Debug.Log("FOUND _start_music_setting");
                _start_music = StartMusic.GetComponent<AudioSource>();
                if (_start_music != null)
                {
                    _start_music_setting = _start_music.volume;
                    _start_music.volume = _start_music_setting * sound_volume;
                    Debug.Log("Updated _start_music_setting");
                }
            }
        }
        if (_bg_music_ingame_setting == 2.0f)
        {
            Debug.Log("Updated _bg_music_ingame_setting called KOK");
            GameObject BgIngameMusic = GameObject.FindGameObjectWithTag("BgIngameMusic");
            if (BgIngameMusic != null)
            {
                Debug.Log("FOUND _bg_music_ingame_setting");
                _bg_music_ingame = BgIngameMusic.GetComponent<AudioSource>();
                if (_bg_music_ingame != null)
                {
                    _bg_music_ingame_setting = _bg_music_ingame.volume;
                    _bg_music_ingame.volume = _bg_music_ingame_setting * sound_volume;
                    Debug.Log("Updated _bg_music_ingame_setting");
                }
            }
        }
        if (_bg_music_menu_setting == 2.0f)
        {
            Debug.Log("Updated _bg_music_menu_setting called KOK");
            GameObject BgMenuMusic = GameObject.FindGameObjectWithTag("BgMenuMusic");
            if (BgMenuMusic != null)
            {
                Debug.Log("FOUND _bg_music_menu_setting");
                _bg_music_menu = BgMenuMusic.GetComponent<AudioSource>();
                if (_bg_music_menu != null)
                {
                    _bg_music_menu_setting = _bg_music_menu.volume;
                    Debug.Log("Updated _bg_music_menu_setting");
                }
            }
        }
    }

    public void PlayBomb()
    {
        bomb.Play();
    }

    /*public void PlayGameStart()
    {
        game_start.Play();
    }*/

    public void Click()
    {
        Debug.Log("button_click volume at: " + button_click.volume);
        button_click.Play();
    }

    public void YouWon()
    {
        Debug.Log("you_won volume at: " + you_won.volume);
        you_won.Play();
    }

    public void YouDied()
    {
        Debug.Log("you_died volume at: " + you_died.volume);
        you_died.Play();
    }

    public void Step()
    {
        Debug.Log("step volume at: " + step.volume);
        step.Play();
    }

    public void StopStep()
    {
        Debug.Log("step volume at: " + step.volume);
        step.Stop();
    }

    public void PlaceBomb()
    {
        Debug.Log("place_bomb volume at: " + place_bomb.volume);
        place_bomb.Play();
    }

    public void CollectItem()
    {
        Debug.Log("collect_item volume at: " + collect_item.volume);
        collect_item.Play();
    }

    public void adjustSound(int new_volume)
    {
        initSoundSettings();
        sound_volume = (float)new_volume / 100;
        bomb.volume = _bomb_sound_setting * sound_volume;
        button_click.volume = _button_click_sound_setting * sound_volume;
        you_won.volume = _you_won_sound_setting * sound_volume;
        step.volume = _step_sound_setting * sound_volume;
        you_died.volume = _you_died_sound_setting * sound_volume;
        place_bomb.volume = _place_bomb_sound_setting * sound_volume;
        collect_item.volume = _collect_item_sound_setting * sound_volume;

        /*if (_start_music != null)
        {
            collect_item.volume = _start_music_setting * sound_volume;
        }
        if (_bg_music_ingame != null)
        {
            _bg_music_ingame.volume = _bg_music_ingame_setting * sound_volume;
        }*/
        if (_bg_music_menu != null)
        {
            _bg_music_menu.volume = _bg_music_menu_setting * sound_volume;
        }

        Debug.Log("new volume value: " + sound_volume);
    }
}
