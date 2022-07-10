using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class PlayerSpawnNotifier : NetworkBehaviour
{
    public static event Action<GameObject> onSpawn;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if(base.IsOwner)
        {
            var netObj = base.LocalConnection.FirstObject;
            if(netObj == base.NetworkObject)
                onSpawn.Invoke(gameObject);
        }
    }
}
