using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class Goblin : Enemy, IDamgeable
{
    public GameObject _arrow;
    public Transform _tip;
    public override void OnNetworkSpawn()
    {
        if(!IsServer) return;

        InitMonster();
    }

    // 몬스터 초기화
    public override void InitMonster()
    {
        if(!IsServer) return;

        if (!stat.isDie)
            _initTransform = this.transform.position;

        else
        {
            _isAttack = false;
            RespawnClientRpc();
            state = States.Idle;
        }

        MaxHp = 1500f;
        Hp = MaxHp;

        stat.attack = 450f;
        stat.attackRange = 7f;
        stat.attackSpeed = 0.5f;

        stat.defense = 250f;

        stat.speed = 1.3f;
        stat.chaseRange = 10f;

        stat.exp = 1000f;
        stat.gold = 1300;

        stat.isDie = false;

        // 원거리 애니메이션
        anim.SetFloat("NormalState", 0.5f);

        StartCoroutine("MonsterState");
    }

    #region 피격 및 사망 처리
    public void Hit(float damage, bool isCritical)
    {
        anim.SetTrigger("Hit");
        StopCoroutine("EnemyAttack");
        _isAttack = false;
        TakeDamageServerRpc(damage, isCritical);
    }

    public override IEnumerator HitEffect()
    {
        yield return null;
    }

    public override void Die()
    {
        if(!IsServer) return;

        Hp = 0f;

        state = States.Die;
        anim.ResetTrigger("Hit");
        anim.SetFloat("RunState", 0f);
        StopAllCoroutines();
    }
    #endregion
    // 이동 애니메이션
    public override void Movement_Anim()
    {
        if(!IsServer) return;

        if(state == States.Chase || state == States.Return)
        {
            anim.SetFloat("RunState", 0.5f);
        }

        else
        {
            anim.SetFloat("RunState", 0f);
        }
    }

    public override IEnumerator EnemyAttack()
    {
        if(!IsServer) yield break;

        while(_isAttack)
        {
            // 공격시 방향 전환 및 애니메이션 실행
            SetDirection();
            anim.SetTrigger("Attack");
            audioController.PlayAttackSFX();

            // 공격속도 지연
            yield return new WaitForSeconds(1 / stat.attackSpeed);

            if(state != States.Attack) 
            {
                _isAttack = false;
                _target = null;
                yield break;
            }

            // 화살 생성 후 타겟 방향으로 회전 및 발사
            NetworkObject arrow = NetworkObjectPool.Instance.GetNetworkObject(_arrow, _tip.position, Quaternion.identity);
            arrow.GetComponent<GoblinArrow>()._enemy = this;
            arrow.GetComponent<GoblinArrow>()._arrow = _arrow;
            Vector3 direction = (_target.position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            arrow.transform.rotation = Quaternion.Euler(0, 0, angle);

            if(!arrow.IsSpawned)
            {
                arrow.Spawn();
            }
            
            arrow.GetComponent<Rigidbody2D>().velocity = direction * 10f;
        }
    }
}
