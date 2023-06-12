using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSound : MonoBehaviour
{
    // Start is called before the first frame update
    public void PlayButtonClickSound()
    {
        SFXPlayer.Instance.Click();
    }
}
