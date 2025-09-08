using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BossEntrance : NetworkBehaviour
{
    [SerializeField]
    private Animator _bossAnim;
    private bool _isStand = false;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if(!IsServer) return;

        if(!_isStand && col.tag == "Player" || col.tag == "EnemyPlayer")
        {
            _isStand = true;
            _bossAnim.SetTrigger("StandUp");
            _bossAnim.GetComponent<Boss>()._isStand = true;
        }
    }
}
