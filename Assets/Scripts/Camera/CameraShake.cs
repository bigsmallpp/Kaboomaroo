using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private float _cameraShakeBaseIntensity;
    private Vector3 _initialCameraPos;
    private float _shakeDuration = 0.25f;
    private float _magnitude;
    private float _currentDuration = 0.0f;
    
    // Start is called before the first frame update
    void Start()
    {
        _initialCameraPos = _camera.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (_currentDuration > 0.0f)
        {
            _camera.transform.position = _initialCameraPos;
        }
    }

    private void LateUpdate()
    {
        if (_currentDuration > 0.0f)
        {
            float x_offset = Random.Range(-_cameraShakeBaseIntensity, _cameraShakeBaseIntensity) * _magnitude;
            float y_offset = Random.Range(-_cameraShakeBaseIntensity, _cameraShakeBaseIntensity) * _magnitude;

            _camera.transform.position = _initialCameraPos + new Vector3(x_offset, y_offset, 0.0f);
            _currentDuration -= Time.deltaTime;
            if (_currentDuration < 0.0f)
            {
                _currentDuration = 0.0f;
                _camera.transform.position = _initialCameraPos;
            }
        }
    }

    public void ShakeCamera()
    {
        _magnitude = SettingsHandler.Instance._currentSettings.camera_shake;
        if (SettingsHandler.Instance._currentSettings.camera_shake == 0.0f)
        {
            return;
        }

        _currentDuration = _shakeDuration;
    }
}
