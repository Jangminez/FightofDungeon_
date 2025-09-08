using Unity.Netcode;


public class SetPlayerLayer : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        if(!IsOwner)
        {
            gameObject.layer = 24;
        }
    }
}
