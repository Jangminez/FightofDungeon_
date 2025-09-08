using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;

public class Bat : Enemy, IDamgeable
{
    [SerializeField] private ScriptableItem _dropItem;

    public override void OnNetworkSpawn()
    {
        spr = GetComponent<SpriteRenderer>();

        if (!IsServer) return;

        InitMonster();
    }

    public override void InitMonster()
    {
        if (!IsServer) return;

        if (!stat.isDie)
            _initTransform = this.transform.position;

        MaxHp = 4000f;
        Hp = MaxHp;

        stat.attack = 500f;
        stat.attackRange = 2f;
        stat.attackSpeed = 2f;

        stat.defense = 100f;

        stat.chaseRange = 6f;
        stat.speed = 2.5f;

        stat.exp = 1200f;
        stat.gold = 1000;

        stat.isDie = false;

        RespawnClientRpc();
        StartCoroutine("MonsterState");
    }

    public override IEnumerator EnemyAttack()
    {
        if (!IsServer) yield break;

        while (_isAttack)
        {
            anim.SetTrigger("Attack");
            audioController.PlayAttackSFX();
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
    public void Hit(float damage, bool isCritical)
    {
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
        Hp = 0f;

        // 몬스터 상태 Die로 설정 후 애니메이션 실행
        state = States.Die;
        anim.SetFloat("RunState", 0f);

        StopAllCoroutines();

        int random_int = Random.Range(1, 101);

        if (random_int <= 5)
        {
            DropItemManager.Instance.DropItemServerRpc(this.transform.position, _dropItem.Id, GetComponent<SortingGroup>().sortingLayerID);
        }
    }

    public override void Movement_Anim()
    {
        if (state == States.Chase || state == States.Return)
        {
            anim.SetFloat("RunState", 0.5f);
        }

        else
        {
            anim.SetFloat("RunState", 0f);
        }
    }
}


