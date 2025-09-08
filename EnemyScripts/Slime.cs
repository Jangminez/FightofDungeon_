using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;

public class Slime : Enemy, IDamgeable
{
    public enum SlimeType { Basic, Fire, Ice };
    public SlimeType slimeType;
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

        else
        {
            _isAttack = false;
            RespawnClientRpc();
            state = States.Idle;
        }

        switch (slimeType)
        {
            case SlimeType.Basic:
                MaxHp = 30f;
                Hp = MaxHp;

                stat.attack = 7f;
                stat.attackRange = 2f;
                stat.attackSpeed = 1.5f;

                stat.defense = 1f;

                stat.chaseRange = 5f;
                stat.speed = 1f;

                stat.exp = 30f;
                stat.gold = 50;
                break;

            case SlimeType.Ice:
                MaxHp = 400f;
                Hp = MaxHp;

                stat.attack = 70f;
                stat.attackRange = 2f;
                stat.attackSpeed = 1.5f;

                stat.defense = 45f;

                stat.chaseRange = 5f;
                stat.speed = 1f;

                stat.exp = 100f;
                stat.gold = 200;
                break;

            case SlimeType.Fire:
                MaxHp = 1000f;
                Hp = MaxHp;

                stat.attack = 350f;
                stat.attackRange = 2f;
                stat.attackSpeed = 1.5f;

                stat.defense = 500f;

                stat.chaseRange = 5f;
                stat.speed = 1.2f;

                stat.exp = 500f;
                stat.gold = 500;
                break;
        }

        stat.isDie = false;

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
        anim.SetTrigger("Hit");
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
        anim.ResetTrigger("Hit");
        anim.SetFloat("RunState", 0f);
        StopAllCoroutines();

        // 5% 확률로 아이템 드랍
        if (slimeType == SlimeType.Fire || slimeType == SlimeType.Ice)
        {
            int random_int = Random.Range(1, 101);

            if (random_int <= 10)
            {
                DropItemManager.Instance.DropItemServerRpc(this.transform.position, _dropItem.Id, GetComponent<SortingGroup>().sortingLayerID);
            }
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

