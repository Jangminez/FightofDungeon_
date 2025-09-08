using Unity.Netcode;
using UnityEngine;

public class PumkinSlash : NetworkBehaviour
{
    public Enemy _enemy;
    public GameObject prefab;
    public Animator anim;

    void OnEnable()
    {

        Invoke("OffSlash", 1.5f);
        if(!IsServer) return;
        OnSlashClientRpc();
    }

    void OnTriggerEnter2D(Collider2D other) 
    {
        if(!IsServer) return;

        if(other.GetComponent<Player>() != null)
        {  
            AttackClientRpc(other.GetComponent<NetworkObject>().OwnerClientId, _enemy.stat.attack);
        }
    }

    void OffSlash()
    {
        this.gameObject.SetActive(false);
    }

    [ClientRpc]
    private void AttackClientRpc(ulong clientId, float damage)
    {
        // 공격 받은 클라이언트라면 Hit() 처리
        if (clientId == NetworkManager.Singleton.LocalClientId)
            GameManager.Instance.player.Hit(damage: damage, false);
    }

    [ClientRpc]
    private void OnSlashClientRpc()
    {
        this.gameObject.SetActive(true);
    }
}
