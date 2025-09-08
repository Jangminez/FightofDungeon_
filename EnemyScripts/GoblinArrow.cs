using Unity.Netcode;
using UnityEngine;

public class GoblinArrow : NetworkBehaviour
{
    public Enemy _enemy;
    public GameObject _arrow;

    void OnEnable()
    {
        OnArrowClientRpc();
    }

    void OnTriggerEnter2D(Collider2D other) 
    {
        if(!IsServer) return;

        if(other.GetComponent<Player>() != null)
        {  
            AttackClientRpc(other.GetComponent<NetworkObject>().OwnerClientId, _enemy.stat.attack);
            NetworkObjectPool.Instance.ReturnNetworkObject(GetComponent<NetworkObject>(), _arrow);
            OffArrowClientRpc();
        }
        else if (other.gameObject.layer == 7)
        {
            NetworkObjectPool.Instance.ReturnNetworkObject(GetComponent<NetworkObject>(), _arrow);
            OffArrowClientRpc();
        }
    }

    [ClientRpc]
    private void AttackClientRpc(ulong clientId, float damage)
    {
        // 공격 받은 클라이언트라면 Hit() 처리
        if (clientId == NetworkManager.Singleton.LocalClientId)
            GameManager.Instance.player.Hit(damage: damage, false);
    }

    [ClientRpc]
    private void OffArrowClientRpc()
    {
        this.gameObject.SetActive(false);
    }

    [ClientRpc]
    private void OnArrowClientRpc()
    {
        this.gameObject.SetActive(true);
    }
}
