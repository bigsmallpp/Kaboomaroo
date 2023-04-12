using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private IPManager _networkManager;
    
    
    [SerializeField] private TextMeshProUGUI _ipText;

    public const string IP = "IP: ";
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WaitForInitAndSetIPAddress());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator WaitForInitAndSetIPAddress()
    {
        while (_networkManager.GetIPStatus() != IPManager.IP_STATUS.SUCCESS)
        {
            yield return new WaitForSeconds(0.5f);
        }
        
        _ipText.text = IP + _networkManager.GetIPAddress();
    }
}
