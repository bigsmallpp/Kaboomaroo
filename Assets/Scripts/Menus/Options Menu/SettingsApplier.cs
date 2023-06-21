using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsApplier : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown _cameraShake;
    [SerializeField] private TMP_Dropdown _resolutionScale;
    [SerializeField] private TMP_Dropdown _frameLimit;
    [SerializeField] private Slider _volume;
    [SerializeField] private Button _buttonApply;


    private Dictionary<float, string> _cameraShakeDict = new Dictionary<float, string>()
    {
        {0.0f, "Off"},
        {0.5f, "Low"},
        {1.5f, "Medium"},
        {4.5f, "JUICY"}
    };
    
    private Dictionary<float, string> _resolutionScaleDict = new Dictionary<float, string>()
    {
        {1.0f, "Full"},
        {0.5f, "Half"},
        {0.25f, "Quarter"}
    };
    public void ApplySettings()
    {
        HideApplyButton();
        
        float camera_shake_multiplier = ConvertCameraShakeToFloat(_cameraShake.options[_cameraShake.value].text);
        float resolution_scale_multiplier = ConvertResolutionScaleToFloat(_resolutionScale.options[_resolutionScale.value].text);
        int frame_limit = ConvertFPSLimitToInt(_frameLimit.options[_frameLimit.value].text);
        int volume = (int)_volume.value;
        Debug.Log("[SOUND] Settings Applier: Sound Volume set to: " + volume);
        SFXPlayer.Instance.adjustSound(volume);

        SettingsSerializable new_settings = new SettingsSerializable(volume, resolution_scale_multiplier, camera_shake_multiplier, frame_limit);
        SettingsHandler.Instance.ApplySettingAndSaveToFile(new_settings);
    }

    private float ConvertCameraShakeToFloat(string value)
    {
        float setting = 0.0f;
        switch (value)
        {
            case "Off":
                setting = 0.0f;
                break;
            
            case "Low":
                setting = 0.5f;
                break;
            
            case "Medium":
                setting = 1.5f;
                break;
            
            case "JUICY":
                setting = 4.5f;
                break;
            
            default:
                setting = 0.0f;
                break;
        }

        return setting;
    }
    
    private float ConvertResolutionScaleToFloat(string value)
    {
        float setting = 1.0f;
        switch (value)
        {
            case "Full":
                setting = 1.0f;
                break;
            
            case "Half":
                setting = 0.5f;
                break;
            
            case "Quarter":
                setting = 0.25f;
                break;

            default:
                setting = 1.0f;
                break;
        }

        return setting;
    }

    private int ConvertFPSLimitToInt(string value)
    {
        int limit = 999;
        switch (value)
        {
            case "Unlimited":
                limit = 999;
                break;
            
            case "200":
            case "120":
            case "90":
            case "60":
                limit = int.Parse(value);
                break;
            
            default:
                limit = 60;
                break;
        }

        return limit;
    }

    public void ShowApplyButton()
    {
        _buttonApply.gameObject.SetActive(true);
    }
    private void HideApplyButton()
    {
        _buttonApply.gameObject.SetActive(false);
    }

    private void SetCurrentSettings(SettingsSerializable settings)
    {
        _volume.value = settings.volume;
        _cameraShake.value = _cameraShake.options.FindIndex(option => option.text == _cameraShakeDict[settings.camera_shake]);
        _resolutionScale.value = _resolutionScale.options.FindIndex(option => option.text == _resolutionScaleDict[settings.screen_resolution]);

        if (settings.frame_limit == 999)
        {
            _frameLimit.value = _frameLimit.options.FindIndex(option => option.text == "Unlimited");
        }
        else
        {
            _frameLimit.value = _frameLimit.options.FindIndex(option => option.text == settings.frame_limit.ToString());
        }
    }

    private void OnEnable()
    {
        SettingsSerializable settings = SettingsHandler.Instance._currentSettings;
        SetCurrentSettings(settings);
        _buttonApply.gameObject.SetActive(false);
    }
}
