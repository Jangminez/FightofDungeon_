using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BossAttack : NetworkBehaviour
{
    public bool _isSpin;
    public float _multiple;
    public float _damage;
    private List<Collider2D> _InRange;

    public override void OnNetworkSpawn()
    {
        if(!IsServer) return;

        StartCoroutine(InitAttack());
    }

    IEnumerator InitAttack()
    {
        if(!IsServer) yield break;

        yield return null;

        _InRange = new List<Collider2D>();
        _damage = transform.parent.GetComponentInParent<Boss>().stat.attack * _multiple;

        yield return new WaitForSeconds(0.1f);
        GetComponent<SpriteRenderer>().enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!IsServer) return;

        if (col.GetComponent<Player>() != null)
        {
            if (_isSpin)
            {
                _InRange.Add(col);
                StartCoroutine(SpinAttack(col));
            }
            else
            {
                AttackClientRpc(col.GetComponent<NetworkObject>().OwnerClientId, _damage);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (IsServer && _isSpin)
        {
            _InRange.Remove(col);
        }
    }

    IEnumerator SpinAttack(Collider2D col)
    {
        if (!IsServer) yield break;

        var enemy = col.GetComponent<IDamgeable>();

        while (enemy != null && _InRange.Contains(col))
        {
            AttackClientRpc(col.GetComponent<NetworkObject>().OwnerClientId, _damage);

            yield return new WaitForSeconds(0.5f);
        }
    }

    [ClientRpc]
    private void AttackClientRpc(ulong clientId, float damage)
    {
        // 공격 받은 클라이언트라면 Hit() 처리
        if (clientId == NetworkManager.Singleton.LocalClientId)
            GameManager.Instance.player.Hit(damage: damage, false);
    }
}
