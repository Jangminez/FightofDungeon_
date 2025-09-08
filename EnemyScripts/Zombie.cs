using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Zombie : Enemy, IDamgeable
{
    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        InitMonster();
    }

    // 몬스터 초기화
    public override void InitMonster()
    {
        if (!IsServer) return;

        if (!stat.isDie)
            _initTransform = this.transform.position;

        else
        {
            _isAttack = false;
            state = States.Idle;
            RespawnClientRpc();

        }

        MaxHp = 700f;
        Hp = MaxHp;

        stat.attack = 100f;
        stat.attackRange = 2f;
        stat.attackSpeed = 0.8f;

        stat.defense = 50f;

        stat.speed = 0.7f;
        stat.chaseRange = 5f;

        stat.exp = 300f;
        stat.gold = 500;

        stat.isDie = false;

        StartCoroutine("MonsterState");
    }

    #region 피격 및 사망 처리
    public void Hit(float damage, bool isCritical)
    {
        TakeDamageServerRpc(damage, isCritical);
    }

    public override IEnumerator HitEffect()
    {
        yield return null;
    }

    public override void Die()
    {
        if (!IsServer) return;

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
        if (!IsServer) return;

        if (state == States.Chase || state == States.Return)
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
        if (!IsServer) yield break;

        while (_isAttack)
        {
            // 공격시 방향 전환 및 애니메이션 실행
            SetDirection();

            int ran = Random.Range(0, 2);

            if (ran == 0)
                anim.SetFloat("NormalState", 0f);
            else
                anim.SetFloat("NormalState", 0.5f);

            anim.SetTrigger("Attack");
            audioController.PlayAttackSFX();

            // 공격속도 지연
            yield return new WaitForSeconds(1 / stat.attackSpeed);

            if (state != States.Attack)
            {
                _isAttack = false;
                _target = null;
                yield break;
            }

            if (_target != null && Vector2.Distance(_target.position, transform.position) < stat.attackRange)
                AttackClientRpc(_target.GetComponent<NetworkObject>().OwnerClientId, stat.attack, false);
        }
    }
}
