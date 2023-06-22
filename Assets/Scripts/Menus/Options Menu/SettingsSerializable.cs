using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SettingsSerializable
{
    public int volume;
    public float screen_resolution;
    public float camera_shake;
    public int frame_limit;

    public SettingsSerializable(int vol, float res, float shake, int limit)
    {
        volume = vol;
        screen_resolution = res;
        camera_shake = shake;
        frame_limit = limit;
    }
    
    // Default Settings
    public SettingsSerializable()
    {
        volume = 80;
        screen_resolution = 1.0f;
        camera_shake = 1.0f;
        frame_limit = 200;
    }
}
