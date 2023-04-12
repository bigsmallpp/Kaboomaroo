using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

public class IPManager : MonoBehaviour
{
    private IPAddress _deviceIPAddress;
    private IP_STATUS _status;
    public enum IP_STATUS
    {
        UNINITIALIZED,
        SUCCESS,
        FAILED,
        IN_PROGRESS
    };
    
    // Start is called before the first frame update
    void Start()
    {
        _status = IP_STATUS.UNINITIALIZED;
        StartCoroutine(SetIPAddress());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    IEnumerator SetIPAddress()
    {
        if (_status == IP_STATUS.IN_PROGRESS)
            yield return null;

        _status = IP_STATUS.IN_PROGRESS;
        UnityWebRequest uwc = UnityWebRequest.Get("http://ipinfo.io/ip");
        yield return uwc.SendWebRequest();

        switch (uwc.result)
        {
            default:
            case UnityWebRequest.Result.ConnectionError:
            case UnityWebRequest.Result.ProtocolError:
            case UnityWebRequest.Result.DataProcessingError:
                _status = IP_STATUS.FAILED;
                break;
            
            case UnityWebRequest.Result.Success:
                string ip_addr = uwc.downloadHandler.text;
                _deviceIPAddress = IPAddress.Parse(ip_addr);
                _status = IP_STATUS.SUCCESS;
                break;
        }
    }

    public string GetIPAddress()
    {
        return _deviceIPAddress.ToString();
    }

    public IP_STATUS GetIPStatus()
    {
        return _status;
    }

    public void RefreshIP()
    {
        StartCoroutine(SetIPAddress());
    }
}
