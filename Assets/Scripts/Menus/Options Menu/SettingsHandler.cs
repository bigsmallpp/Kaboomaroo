using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SettingsHandler : MonoBehaviour
{
    private static SettingsHandler _instance;
    public static SettingsHandler Instance => _instance;
    
    public SettingsSerializable _currentSettings = null;
    public const string SAVE_FILE = "KaboomarooSettings.json"; 
    
    // Start is called before the first frame update
    void Start()
    {
        if (_instance == null)
        {
            _instance = this;
            LoadSettingsFromFile();
            ApplySettings(_currentSettings);
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this);
        }
    }

    private void LoadSettingsFromFile()
    {
        try
        {
            string contents = File.ReadAllText(SAVE_FILE);
            SettingsSerializable settings = JsonUtility.FromJson<SettingsSerializable>(contents);
            _currentSettings = settings;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            Debug.LogError("Error when trying to load settings, creating new Default Settings");
            _currentSettings = new SettingsSerializable();
        }
    }

    public void SaveSettingsToFile(SettingsSerializable new_settings)
    {
        _currentSettings = new_settings;

        try
        {
            string content = JsonUtility.ToJson(_currentSettings);
            System.IO.File.WriteAllText(SAVE_FILE, content);
        }
        catch (Exception e)
        {
            Debug.LogError("Error - Couldn't save to file");
            Debug.LogError(e);
        }
    }

    public void ApplySettingAndSaveToFile(SettingsSerializable new_settings)
    {
        ApplySettings(new_settings);
        SaveSettingsToFile(new_settings);
    }

    private void ApplySettings(SettingsSerializable new_settings)
    {
        // TODO Adjust Volume in SoundManager
        Application.targetFrameRate = new_settings.frame_limit;

        Resolution max_res = Screen.resolutions[Screen.resolutions.Length - 1];
        int width = (int)(max_res.width * new_settings.screen_resolution);
        int height = (int)(max_res.height * new_settings.screen_resolution);
        Screen.SetResolution(width > height ? width : height, width > height ? height : width, true);
    }
}
