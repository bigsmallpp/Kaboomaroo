using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class NetworkedGameMenus : NetworkBehaviour
{
    public UnityEventBool onShowDeathScreen;
    
    [ClientRpc]
    public void RPC_SwitchToDeathMessageClientRPC(ulong client_id)
    {
        if (client_id != NetworkManager.Singleton.LocalClientId)
        {
            return;
        }
        
        onShowDeathScreen.Invoke(true);
    }
}
