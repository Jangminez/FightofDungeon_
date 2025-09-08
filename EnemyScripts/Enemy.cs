using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public abstract class Enemy : NetworkBehaviour
{
    #region 참조 변수

    public GameObject prefab;
    protected SpriteRenderer spr;
    protected Rigidbody2D rb;
    protected Animator anim;
    public AudioController audioController;
    public GameObject FloatingDamagePrefab;
    public GameObject FloatingGoldExpPrefab;

    #endregion

    #region 상태 및 위치 변수
    public Vector3 _initTransform;
    public enum States { Idle, Chase, Attack, Return, Die }
    public States state;

    public Transform _target;
    protected bool _isAttack;
    Transform _canvas;
    Vector3 _initCanvasScale;
    #endregion

    #region 적 스탯 변수
    [Serializable]
    public struct Stats
    {
        public float attack;
        public float attackSpeed;
        public float defense;
        public float speed;
        public float attackRange;
        public float chaseRange;
        public float exp;
        public int gold;
        public bool isDie;
    }

    public Stats stat;

    // 몬스터의 체력에 대한 NetworkVariable
    private NetworkVariable<float> _maxHp = new NetworkVariable<float>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<float> _hp = new NetworkVariable<float>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public float MaxHp
    {
        set => _maxHp.Value = Mathf.Max(0, value);
        get => _maxHp.Value;
    }

    public float Hp
    {
        set
        {
            if (_hp.Value >= 0 && _hp.Value != value)
            {
                _hp.Value = Mathf.Max(0, value);
            }

        }
        get => _hp.Value;
    }
    #endregion

    private void Awake()
    {
        // 참조 변수 적용
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        _canvas = transform.GetChild(0);
        _initCanvasScale = _canvas.transform.localScale;
        audioController = GetComponent<AudioController>();
    }

    private void Start()
    {
        // 체력바 UI 표시를 위한 이벤트 연결
        _hp.OnValueChanged += GetComponent<EnemyHp>().ChangeHp;
    }

    void FixedUpdate()
    {
        // 서버에서 적의 공격 및 애니메이션 관리
        if (!IsServer) return;

        Movement_Anim();

        // 공격 여부 판정
        if (!_isAttack && state == States.Attack)
        {
            _isAttack = true;
            anim.SetFloat("AttackSpeed", stat.attackSpeed);
            StartCoroutine("EnemyAttack");
        }
    }

    virtual public IEnumerator MonsterState()
    {
        // 몬스터 상태(Idle, Chase, Attack, Return)
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

                if (_target != null && Vector2.Distance(_target.position, transform.position) > stat.attackRange && !stat.isDie)
                {
                    state = States.Chase;
                }
            }

            // 초기위치로 돌아감
            else if (state == States.Return)
            {
                Movement(_initTransform);

                if (Vector3.Distance(_initTransform, this.transform.position) < 0.1f)
                {
                    state = States.Idle;
                }
            }
        }
    }
    
    // 몬스터 초기화 함수
    public abstract void InitMonster();
    #region 적 이동 관련 함수
    public virtual void Movement(Vector3 target)
    {
        if (!IsServer) return;

        if (target == null)
        {
            state = States.Idle;
            return;
        }

        Vector2 dirVec = target - transform.position;
        Vector2 nextVec = dirVec.normalized * stat.speed * Time.fixedDeltaTime;

        rb.MovePosition(rb.position + nextVec);
    }
    public abstract void Movement_Anim();
    public virtual void SetDirection()
    {
        if (_target != null && _target.position.x - transform.position.x > 0)
        {
            if (anim.transform.localScale.x < 0)
                return;

            else
            {
                anim.transform.localScale = new Vector3(-anim.transform.localScale.x, anim.transform.localScale.y, 1f);
                _canvas.localScale = new Vector3(-_initCanvasScale.x, _initCanvasScale.y, _initCanvasScale.z);
            }
        }

        else if (_target != null && _target.position.x - transform.position.x < 0)
        {
            if (anim.transform.localScale.x < 0)
            {
                anim.transform.localScale = new Vector3(-anim.transform.localScale.x, anim.transform.localScale.y, 1f);
                _canvas.localScale = _initCanvasScale;
            }

            else
                return;
        }
    }
    virtual public void OutofArea()
    {
        state = States.Return;
    }
    #endregion
    
    #region 적 전투 관련 함수 (공격, 피격, 사망)
    public abstract IEnumerator EnemyAttack();
    public abstract IEnumerator HitEffect();
     public abstract void Die();

    #endregion

    #region 서버 처리 관련(ServerRpc, ClientRpc)

    [ServerRpc(RequireOwnership = false)]
    public void GiveExpGoldServerRpc(ulong lastClientId)
    {
        if(!stat.isDie)
        {
            ShowGoldClientRpc(lastClientId, stat.gold, stat.exp);
            stat.isDie = true;
        }
    }

    [ClientRpc]
    protected void AttackClientRpc(ulong clientId, float damage, bool isCritical)
    {
        // 공격 받은 클라이언트라면 Hit() 처리
        if (clientId == NetworkManager.Singleton.LocalClientId)
            GameManager.Instance.player.Hit(damage: damage, isCritical);
    }

    [ClientRpc]
    public void ShowFloatingDamageClientRpc(float damage, bool isCritical)
    {
        // 몬스터 피격 데미지 표시
        var dmg = Instantiate(FloatingDamagePrefab, transform.position, Quaternion.identity);
        if(isCritical)
            dmg.GetComponent<TextMesh>().color = Color.red;

        dmg.GetComponent<TextMesh>().text = $"-" + damage.ToString("F1");
    }

    [ClientRpc]
    public virtual void ShowGoldClientRpc(ulong clientId, int gold, float exp)
    {
        // 마지막으로 처치한 클라이언트라면 골드와 경험치 지급
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            GameManager.Instance.player.Gold += gold;
            GameManager.Instance.player.Exp += exp;
            var floating = Instantiate(FloatingGoldExpPrefab, transform.position, Quaternion.identity);
            floating.GetComponent<TextMesh>().text = $"\n+{gold}Gold";
        }
    }

    [ServerRpc(RequireOwnership = false)]
    protected void TakeDamageServerRpc(float damage, bool isCritical, ServerRpcParams rpcParams = default)
    {
        // 받은 데미지 - 방어력 으로 최종데미지 계산
        float finalDamage = damage - stat.defense;

        if (finalDamage < 0f)
        {
            finalDamage = 1f;
        }

        Hp -= finalDamage;

        audioController.PlayHitSFX();

        if (FloatingDamagePrefab != null && Hp > 0)
        {
            // 데미지 표시 동기화
            ShowFloatingDamageClientRpc(finalDamage, isCritical);
        }

        if (Hp <= 0)
        {
            // 체력이 0 이하라면 Die
            StopAllCoroutines();

            Die();
    
            StartCoroutine(DeSpawnEnemy(GetComponent<NetworkObject>(), prefab, 3f));
            DieClientRpc(rpcParams.Receive.SenderClientId);
            anim.SetTrigger("Die");
        }
    }

    [ClientRpc]
    protected void DieClientRpc(ulong lastAttackClient)
    {
        audioController.audioSource.Stop();
        
        audioController.PlayDeathSFX();

        // 처치한 클라이언트에게 경험치랑 골드 지급
        if (NetworkManager.Singleton.LocalClientId == lastAttackClient)
            GiveExpGoldServerRpc(lastAttackClient);

        GetComponent<Collider2D>().enabled = false;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        GetComponent<Animator>().ResetTrigger("Respawn");
    }

    [ClientRpc]
    protected void ActiveFalseClientRpc()
    {
        gameObject.SetActive(false);
    }

    [ClientRpc]
    protected void RespawnClientRpc()
    {
        GetComponent<Collider2D>().enabled = true;
        gameObject.SetActive(true);
        anim.SetTrigger("Respawn");
    }

    #endregion
    IEnumerator DeSpawnEnemy(NetworkObject obj, GameObject prefab, float time)
    {
        yield return new WaitForSeconds(time);
        ActiveFalseClientRpc();
        NetworkMonsterSpawner.Instance.DespawnMonster(obj, prefab);
    }
}


