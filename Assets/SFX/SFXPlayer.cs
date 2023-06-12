using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXPlayer : MonoBehaviour
{
    public AudioSource bomb;
    public AudioSource button_click;
    public AudioSource you_won;
    public AudioSource step;
    public AudioSource you_died;
    public AudioSource place_bomb;
    public AudioSource collect_item;

    public int callCount = 0;

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
    public SoundScript _bg_music_menu_script;

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
        CheckForBGSound();
    }

    private void CheckForBGSound()
    {
        GameObject BgMenuMusic = GameObject.FindGameObjectWithTag("BgMenuMusic");
        if (BgMenuMusic != null)
        {
            if (_bg_music_menu_setting == 2.0f)
            {
                _bg_music_menu_setting = BgMenuMusic.GetComponent<AudioSource>().volume;
            }
            BgMenuMusic.GetComponent<SoundScript>().StartSound(_bg_music_menu_setting);
        }
    }

    private void initSoundSettings()
    {
        if (_bomb_sound_setting == 2.0f && bomb != null)
        {
            _bomb_sound_setting = bomb.volume;
        }
        if (_button_click_sound_setting == 2.0f && button_click != null)
        {
            _button_click_sound_setting = button_click.volume;
        }
        if (_you_won_sound_setting == 2.0f && you_won != null)
        {
            _you_won_sound_setting = you_won.volume;
        }
        if (_step_sound_setting == 2.0f && step != null)
        {
            _step_sound_setting = step.volume;
        }
        if (_you_died_sound_setting == 2.0f && you_died != null)
        {
            _you_died_sound_setting = you_died.volume;
        }
        if (_place_bomb_sound_setting == 2.0f && place_bomb != null)
        {
            _place_bomb_sound_setting = place_bomb.volume;
        }
        if (_collect_item_sound_setting == 2.0f && collect_item != null)
        {
            _collect_item_sound_setting = collect_item.volume;
        }
    }

    public void PlayBomb()
    {
        bomb.Play();
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

        GameObject BgMenuMusic = GameObject.FindGameObjectWithTag("BgMenuMusic");
        _bg_music_menu = BgMenuMusic.GetComponent<AudioSource>();
        _bg_music_menu_script = BgMenuMusic.GetComponent<SoundScript>();
        if (_bg_music_menu != null && _bg_music_menu_script != null)
        {
            _bg_music_menu.volume = _bg_music_menu_script.soundSetting * sound_volume;
        }
        else
        {
            Debug.LogError("_bg_music_menu and _bg_music_menu_script are null");
        }
    }
}
