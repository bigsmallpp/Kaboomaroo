using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundScript : MonoBehaviour
{
    public float soundSetting;
    public AudioSource audioSource;
    public bool isMenu;
    // Start is called before the first frame update
    private void Start()
    {
        if (!isMenu)
        {
            audioSource = GetComponent<AudioSource>();
            soundSetting = audioSource.volume;
            audioSource.volume = soundSetting * SFXPlayer.Instance.sound_volume;
            audioSource.Play();
        }
    }
    public void StartSound(float base_volume)
    {
        SFXPlayer.Instance.callCount++;
        audioSource = GetComponent<AudioSource>();
        soundSetting = base_volume;
        Debug.Log("Sound Setting: " + base_volume);
        setSoundSetting();
        audioSource.Play();
    }

    public void setSoundSetting()
    {
        Debug.Log("Volume setting SFXPlayer: " + SFXPlayer.Instance.sound_volume);
        audioSource.volume = soundSetting * SFXPlayer.Instance.sound_volume;
        Debug.Log("New Volume: " + audioSource.volume);
    }
}
