using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Pumkin : Enemy, IDamgeable
{
    public GameObject _batPrefab;
    public GameObject _attackIndicator;
    public GameObject _attackEffect;
    bool _indiOn = false;

    public override void OnNetworkSpawn()
    {
        stat.chaseRange = 6f;

        if(!IsServer) return;

        InitMonster();
    }

    // 몬스터 초기화
    public override void InitMonster()
    {
        if (!IsServer) return;

        if (!stat.isDie)
        {
            // 몬스터가 처음 생성 되었다면 초기 설정
            _initTransform = this.transform.position;   
        }

        else
        {
            // 다시 부활 시 초기 설정
            _isAttack = false;
            _indiOn = false;
            RespawnClientRpc();
            state = States.Idle;
        }

        //몬스터의 초기 스탯
        MaxHp = 5000f;
        Hp = MaxHp;

        stat.attack = 1000f;
        stat.attackRange = 3.5f;
        stat.attackSpeed = 1.2f;

        stat.defense = 300f;

        stat.speed = 1.5f;
        stat.chaseRange = 6f;

        stat.exp = 2500f;
        stat.gold = 3000;

        stat.isDie = false;

        // 몬스터 관리 FSM 실행
        StartCoroutine("MonsterState");
    }

    #region 피격 및 사망 처리
    public void Hit(float damage, bool isCritical)
    {
        // 데미지 처리 ServerRpc 호출
        TakeDamageServerRpc(damage, isCritical);
    }

    public override IEnumerator HitEffect()
    {
        return null;
    }

    public override void Die()
    {
        // 몬스터 사망 시 처리 로직
        if (!IsServer) return;

        Hp = 0f;

        state = States.Die;

        OffAttackIndicatorClientRpc();

        int random_count = Random.Range(1, 4);

        StartCoroutine(SpawnBats(random_count));

        anim.ResetTrigger("Hit");
        anim.SetFloat("RunState", 0f);
    }

    IEnumerator SpawnBats(int count)
    {
        yield return new WaitForSeconds(1f);

        for(int i = 0; i < count; i++)
        {
            // 랜덤 개수로 박쥐 소환
            NetworkObject Bat = NetworkObjectPool.Instance.GetNetworkObject(_batPrefab, transform.position, Quaternion.identity);
            Bat.GetComponent<Enemy>().prefab = _batPrefab;

            if(!Bat.IsSpawned)
            {
                Bat.Spawn();
            }
            Bat.GetComponent<Enemy>().InitMonster();
        }
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
            while (anim.GetCurrentAnimatorStateInfo(2).IsName("2_Attack_Normal_pum"))
            {
                yield return null;
            }

            if (!_indiOn)
            {
                SetDirection();
                _indiOn = true;
                OnAttackIndicatorClientRpc();
            }

            yield return new WaitForSeconds(1 / stat.attackSpeed);

            if (state != States.Attack)
            {
                _isAttack = false;
                _target = null;
                yield break;
            }
        }
    }

    public override IEnumerator MonsterState()
    {
        if (!IsServer) yield break;

        while (!stat.isDie)
        {
            yield return null;

            if (_target == null && state != States.Idle && state != States.Return)
            {
                state = States.Idle;
            }

            if (state == States.Idle)
            {
                rb.velocity = Vector2.zero;

                if (_target != null && Vector2.Distance(_target.position, transform.position) < stat.chaseRange && !stat.isDie)
                {
                    state = States.Chase;
                }
            }

            else if (state == States.Chase)
            {
                if (_target == null)
                    state = States.Idle;

                // 타겟의 위치 확인 후 이동
                Movement(_target.position);
                SetDirection();

                if (Vector2.Distance(_target.position, transform.position) < stat.attackRange && !stat.isDie)
                {
                    state = States.Attack;
                }

                else if (Vector2.Distance(_target.position, transform.position) > stat.chaseRange && !stat.isDie)
                {
                    state = States.Idle;
                }

            }

            else if (state == States.Attack)
            {
                rb.velocity = Vector2.zero;

                if (_target == null)
                {
                    state = States.Idle;
                }

                if (_target != null && Vector2.Distance(_target.position, transform.position) > stat.attackRange && !_isAttack && !stat.isDie)
                {
                    state = States.Chase;
                }
            }

            // 초기위치로 돌아감
            else if (state == States.Return)
            {
                Vector2 dirVec = _initTransform - this.transform.position;
                Vector2 nextVec = dirVec.normalized * stat.speed * Time.fixedDeltaTime;

                rb.MovePosition(rb.position + nextVec);

                if (Vector3.Distance(_initTransform, this.transform.position) < 0.1f)
                {
                    state = States.Idle;
                }
            }
        }
    }


    private IEnumerator SetIndicator(Transform target)
    {
        // 인디케이터 방향 설정
        if (target == null) yield break;

        Vector3 direction = (target.position - _attackIndicator.transform.position).normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        _attackIndicator.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90f));

        // 공격까지 대기
        float elapsedTime = 0f;
        float duration = 1 / stat.attackSpeed;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        if(!IsServer) yield break;

        StartCoroutine(SlashAttack(angle, direction));
        OffAttackIndicatorClientRpc();
    }

    private IEnumerator SlashAttack(float angle, Vector3 direction)
    {
        if(!IsServer) yield break;  
        
        // 시간이 지나면 공격
        NetworkObject slash = NetworkObjectPool.Instance.GetNetworkObject(_attackEffect, transform.position, Quaternion.identity);
        
        slash.GetComponent<PumkinSlash>()._enemy = this;
        slash.GetComponent<PumkinSlash>().prefab = _attackEffect;
        slash.transform.rotation = Quaternion.Euler(new Vector3(0,0, angle));

        if(!slash.IsSpawned)
        {
            slash.Spawn();
        }

        slash.GetComponent<Rigidbody2D>().velocity = direction * 3f;
        anim.SetTrigger("Attack");
        audioController.PlayAttackSFX();

        yield return new WaitForSeconds(1f);
        NetworkObjectPool.Instance.ReturnNetworkObject(slash, _attackEffect);
        _indiOn = false;
    }

    [ClientRpc]
    private void OnAttackIndicatorClientRpc()
    {
        _attackIndicator.SetActive(true);
        StartCoroutine(SetIndicator(_target));
    }

    [ClientRpc]
    private void OffAttackIndicatorClientRpc()
    {
        _attackIndicator.SetActive(false);
    }
}
