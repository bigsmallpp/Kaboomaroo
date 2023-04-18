using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class TextOnClickHandler : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TextMeshProUGUI _text;

    public void OnPointerClick(PointerEventData data)
    {
        TextEditor text_editor = new TextEditor();
        text_editor.text = _text.text.Substring(_text.text.Length - 6);
        text_editor.SelectAll();
        text_editor.Copy();

        if (Application.platform == RuntimePlatform.Android)
        {
            // From https://stackoverflow.com/questions/52590525/how-to-show-a-toast-message-in-unity-similar-to-one-in-android
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            if (unityActivity != null)
            {
                AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
                unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
                {
                    AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity, "Code copied to clipboard!", 0);
                    toastObject.Call("show");
                }));
            }
        }
    }
}
