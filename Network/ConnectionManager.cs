using Unity.Netcode;
using UnityEngine;

public class ConnectionManager : NetworkBehaviour
{
    void Start()
    {   
        if(IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        }
        else if(IsClient)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += OnDisconnected;
        }
    }

    void OnClientDisconnected(ulong clinetId)
    {
        if(NetworkManager.Singleton.ConnectedClientsList.Count == 1)
        {
            StageRewardManager.Instance.ShowRewardUI(true);
        }
    }

    void OnDisconnected(ulong clientId)
    {
        if(NetworkManager.LocalClientId != clientId)
        StageRewardManager.Instance.ShowRewardUI(true);
    }
}
