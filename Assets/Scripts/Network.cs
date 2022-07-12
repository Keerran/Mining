using System.Collections;
using System.Collections.Generic;
using FishNet.Managing;
using UnityEngine;

public class Network : MonoBehaviour
{
    private NetworkManager _networkManager;

    void Awake()
    {
        _networkManager = GetComponent<NetworkManager>();
    }

    void OnApplicationQuit()
    {
        _networkManager.ServerManager.StopConnection(true);
    }
}
