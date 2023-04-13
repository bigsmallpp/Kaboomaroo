using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class LoadingAnimation : MonoBehaviour
{
    [SerializeField] private float _interval;
    [SerializeField] private TextMeshProUGUI _connectingText;
    private void OnEnable()
    {
        _connectingText.text = "Connecting";
        StartCoroutine(PlayAnimation());
    }

    private void OnDisable()
    {
        StopCoroutine(PlayAnimation());
    }

    IEnumerator PlayAnimation()
    {
        float passed_time = 0.0f;
        int amount_dots = 0;
        while (true)
        {
            passed_time += Time.deltaTime;
            if (passed_time >= _interval)
            {
                amount_dots = amount_dots % 4;
                _connectingText.text = "Connecting" + String.Concat(Enumerable.Repeat(".", amount_dots));
                amount_dots++;
                passed_time = 0.0f;
            }

            yield return null;
        }
    }
}
