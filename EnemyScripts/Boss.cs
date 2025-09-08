using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Boss : Enemy, IDamgeable
{
    public bool _isPattern;
    public bool _isStand;
    private float r_Pattern;

    [SerializeField]
    private ulong _lastAttackClientId;
    [SerializeField] private StageTimer stageTimer;

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
            return;

        InitMonster();
    }

    public override void InitMonster()
    {
        if (!IsServer)
            return;

        _initTransform = transform.position;

        _isPattern = false;

        MaxHp = 15000f;
        Hp = MaxHp;

        stat.attack = 2000f;
        stat.attackRange = 6f;
        stat.attackSpeed = 1f;

        stat.defense = 2000f;

        stat.speed = 1.5f;
        stat.chaseRange = 12f;

        stat.isDie = false;

        anim.SetFloat("RunState", 0f);

        state = States.Idle;
        StartCoroutine("MonsterState");
    }

    public override IEnumerator MonsterState()
    {
        if (!IsServer)
            yield break;

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
            else if (state == States.Chase && _isStand)
            {
                if (_target == null)
                    state = States.Idle;

                // 타겟의 위치 확인 후 이동
                Movement(_target.position);
                SetDirection();

                if (
                    Vector2.Distance(_target.position, transform.position) < stat.attackRange && !stat.isDie)
                {
                    state = States.Attack;
                }
                else if (
                    Vector2.Distance(_target.position, transform.position) > stat.chaseRange && !stat.isDie)
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

                if (
                    _target != null
                    && Vector2.Distance(_target.position, transform.position) > stat.attackRange
                    && !stat.isDie
                )
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

                if (Vector3.Distance(_initTransform, this.transform.position) < 0.5f)
                {
                    state = States.Idle;
                }
            }
        }
    }

    public override void Die()
    {
        if (!IsServer)
            return;

        state = States.Die;
        StopAllCoroutines();

        // 이긴 클라이언트 ID 
        EndGameClientRpc(_lastAttackClientId);

        if(stageTimer != null)
        {
            stageTimer.EndStage(true);
        }
    }

    public override IEnumerator EnemyAttack()
    {
        if (!IsServer) yield break;
        
        // 공격시 방향 전환 및 애니메이션 실행
        SetDirection();

        r_Pattern = Random.Range(0f, 101f);

        if (r_Pattern <= 50f)
        {
            // 기본 공격
            _isPattern = true;
            StartCoroutine(Boss_BasicAttack());
        }

        else if (50f < r_Pattern && r_Pattern <= 80f)
        {
            // 점프공격
            _isPattern = true;
            StartCoroutine(Boss_JumpAttack());
        }

        else
        {
            // 회전공격
            _isPattern = true;
            StartCoroutine(Boss_SpinAttack());
        }

        if (state != States.Attack)
        {
            _isAttack = false;
            _target = null;
            yield break;
        }
    }

    public void Hit(float damage, bool isCritical)
    {
        TakeDamageServerRpc(damage, isCritical);
        SetLastAttackClientServerRpc();
    }

    public override IEnumerator HitEffect()
    {
        yield break;
    }

    public override void Movement_Anim()
    {
        if (!IsServer)
            return;


        if (state == States.Chase || state == States.Return || state == States.Attack)
        {
            anim.SetFloat("RunState", 1f);
        }
        else
        {
            anim.SetFloat("RunState", 0f);
        }
    }

    private IEnumerator Boss_BasicAttack()
    {
        anim.SetTrigger("Attack");

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName("Attack")
            && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.6f);

        audioController.PlayAttackSFX();
        _isAttack = false;
    }

    private IEnumerator Boss_JumpAttack()
    {
        anim.SetTrigger("JumpAttack");
        stat.speed = 0f;

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName("JumpAttack")
            && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f);

        audioController.PlaySkill1SFX();
        GetComponent<Collider2D>().enabled = false;
        stat.attackRange = 0f;
        stat.speed = 5f;

        yield return new WaitForSeconds(2f);

        stat.speed = 0f;
        anim.SetTrigger("JumpDown");

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName("JumpDown")
            && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.3f);

        audioController.PlaySkill2SFX();
        GetComponent<Collider2D>().enabled = true;

        yield return new WaitForSeconds(0.5f);

        stat.attackRange = 5f;
        stat.speed = 1.2f;

        _isAttack = false;
    }

    private IEnumerator Boss_SpinAttack()
    {
        anim.SetTrigger("SpinAttack");
        audioController.PlaySkill3SFX();
        
        yield return new WaitForSeconds(4f);

        _isAttack = false;
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetLastAttackClientServerRpc(ServerRpcParams rpcParams = default)
    {
        // 마지막으로 공격한 클라이언트의 아이디 저장
        _lastAttackClientId = rpcParams.Receive.SenderClientId;
    }

    [ClientRpc]
    private void EndGameClientRpc(ulong lastAttackClient)
    {
        if (lastAttackClient == NetworkManager.Singleton.LocalClientId)
        {
            Debug.Log("Win!!!!!!!!!!");
            StageRewardManager.Instance.ShowRewardUI(true);
        }

        else
        {
            Debug.Log("Lose............");
            StageRewardManager.Instance.ShowRewardUI(false);
        }
    }
}
