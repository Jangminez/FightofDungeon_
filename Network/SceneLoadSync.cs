using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SceneLoadSync : NetworkBehaviour
{
    public NetworkVariable<bool> IsPlayerReady = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public static SceneLoadSync Instance;

    private void Start()
    {
        if(IsOwner)
        {
            Instance = this;
        }   
    }

    public void SetReady()
    {
        IsPlayerReady.Value = true;
    }
}
