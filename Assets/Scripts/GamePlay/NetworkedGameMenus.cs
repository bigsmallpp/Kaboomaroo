using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class NetworkedGameMenus : NetworkBehaviour
{
    public UnityEventBool onShowDeathScreen;
    public UnityEventBool onShowWinnerScreen;
    
    [ClientRpc]
    public void RPC_SwitchToDeathMessageClientRPC(ulong client_id)
    {
        if (client_id != NetworkManager.Singleton.LocalClientId)
        {
            return;
        }
        
        onShowDeathScreen.Invoke(true);
    }

    [ClientRpc]
    public void RPC_SwitchToWinnerMessageClientRPC(ulong client_id)
    {
        if (client_id != NetworkManager.Singleton.LocalClientId)
        {
            return;
        }

        SFXPlayer.Instance.YouWon();
        onShowWinnerScreen.Invoke(true);
    }
}
