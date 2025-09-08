using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public abstract class Player : NetworkBehaviour, IDamgeable
{
    #region 플레이어 참조 변수
    [SerializeField] protected GameObject _floatingDamage;
    [SerializeField] protected Rigidbody2D _playerRig;

    [SerializeField] protected Animator _animator;
    [SerializeField] public AudioController _audio;
    #endregion
    #region 플레이어 스탯 변수
    [Header("Player Stats")]
    [SerializeField] NetworkVariable<float> _maxHp = new NetworkVariable<float>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [SerializeField] NetworkVariable<float> _hp = new NetworkVariable<float>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [SerializeField] private float _hpBonus;
    [SerializeField] private float _hpRegen;
    [SerializeField] private float _hpRegenBonus;

    [Space(10f)]
    [SerializeField] NetworkVariable<float> _maxMp = new NetworkVariable<float>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [SerializeField] NetworkVariable<float> _mp = new NetworkVariable<float>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [SerializeField] private float _mpBonus;
    [SerializeField] private float _mpRegen;
    [SerializeField] private float _mpRegenBonus;

    [Space(10f)]
    [SerializeField] private float _attack;
    [SerializeField] private float _attackBonus;
    [SerializeField] private float _attackSpeed;
    [SerializeField] private float _asBonus;
    [SerializeField] private float _finalAs;
    [SerializeField] private float _critical;
    [Space(10f)]
    [SerializeField] private float _defense;
    [SerializeField] private float _defenseBonus;

    [Space(10f)]
    [SerializeField] private float _speed;
    [SerializeField] private float _speedBonus;

    [Space(10f)]
    [SerializeField] private float _attackRange;

    [Space(10f)]
    [SerializeField] private bool _isDie;

    [Space(10f)]
    [SerializeField] private int _level;

    [SerializeField] private float _exp;
    [SerializeField] private float _nextExp;
    [SerializeField] private int _lvPoint;

    [Space(10f)]
    [SerializeField] private int _gold;

    [Space(10f)]
    public Transform _target;
    [Space(10f)]
    public Transform _spawnPoint;

    #endregion
    private void Start()
    {
        // 체력과 마나 이벤트 연결
        _hp.OnValueChanged += GetComponent<PlayerUIController>().HpChanged;
        _maxHp.OnValueChanged += GetComponent<PlayerUIController>().MaxHpChanged;
        _mp.OnValueChanged += GetComponent<PlayerUIController>().MpChanged;
        _maxMp.OnValueChanged += GetComponent<PlayerUIController>().MaxHpChanged;
        
        _audio = GetComponent<AudioController>();
    }
    // 플레이어 초기화 함수
    abstract protected void SetCharater();

    #region 플레이어 스탯 프로퍼티
    public float MaxHp
    {
        set
        {
            if (_maxHp.Value != value)
            {
                _maxHp.Value = Mathf.Max(0, value);
            }

        }
        get => _maxHp.Value;
    }

    public float Hp
    {
        set
        {
            if (_hp.Value != value)
            {
                _hp.Value = Mathf.Max(0, value);
            }

            if (value >= FinalHp)
            {
                _hp.Value = FinalHp;
            }

        }
        get => _hp.Value;
    }

    public float HpBonus
    {
        set => _hpBonus = Mathf.Max(0, value);
        get => _hpBonus;
    }

    public float HpRegen
    {
        set => _hpRegen = Mathf.Max(0, value);
        get => _hpRegen;
    }

    public float HpRegenBonus
    {
        set => _hpRegenBonus = Mathf.Max(0, value);
        get => _hpRegenBonus;
    }

    public float MaxMp
    {
        set
        {
            if (_maxMp.Value != value)
            {
                _maxMp.Value = Mathf.Max(0, value);
            }
        }
        get => _maxMp.Value;
    }

    public float Mp
    {
        set
        {
            if (_mp.Value != value)
            {
                _mp.Value = Mathf.Max(0, value);
            }

            if (value > FinalMp)
            {
                _mp.Value = FinalMp;
            }
        }
        get => _mp.Value;
    }

    public float MpBonus
    {
        set => _mpBonus = Mathf.Max(0, value);
        get => _mpBonus;
    }

    public float MpRegen
    {
        set => _mpRegen = Mathf.Max(0, value);
        get => _mpRegen;
    }

    public float MpRegenBonus
    {
        set => _mpRegenBonus = Mathf.Max(0, value);
        get => _mpRegenBonus;
    }

    public float Attack
    {
        set => _attack = Mathf.Max(0, value);
        get => _attack;
    }

    public float AttackBonus
    {
        set => _attackBonus = Mathf.Max(0, value);
        get => _attackBonus;
    }

    public float AttackSpeed
    {
        set => _attackSpeed = Mathf.Max(0, value);
        get => _attackSpeed;
    }

    public float AsBonus
    {
        set => _asBonus = Mathf.Max(0, value);
        get => _asBonus;
    }

    public float Critical
    {
        set => _critical = Mathf.Max(0, value);
        get => _critical;
    }

    public float Defense
    {
        set => _defense = Mathf.Max(0, value);
        get => _defense;
    }

    public float DefenseBonus
    {
        set => _defenseBonus = Mathf.Max(0, value);
        get => _defenseBonus;
    }

    public int Gold
    {
        set
        {
            if (_gold != value)
            {
                _gold = Mathf.Max(0, value);
                UIManager.Instance.GoldChanged();
            }
        }
        get => _gold;
    }

    public float NextExp
    {
        set => _nextExp = Mathf.Round(Mathf.Max(0, value));
        get => _nextExp;
    }

    public float Exp
    {
        set
        {
            if (_exp != value)
            {
                _exp = Mathf.Round(Mathf.Max(0, value));
                UIManager.Instance.ExpChanged();

                if (_exp >= _nextExp)
                {
                    LevelUp();
                }
            }
        }

        get => _exp;
    }

    public int LvPoint
    {
        set
        {
            if (LvPoint != value)
            {
                _lvPoint = Mathf.Max(0, value);
                UIManager.Instance.LvPointChange();
            }
        }
        get => _lvPoint;
    }

    public int Level
    {
        set
        {
            if (_level != value)
            {
                _level = Mathf.Max(0, value);
                UIManager.Instance.LevelChanged();
                StageRewardManager.Instance.playerLevel = _level;
            }
        }

        get => _level;
    }

    public float Speed
    {
        set
        {
            _speed = Mathf.Max(0, value);
            GetComponent<PlayerMovement>()._speed = FinalSpeed;
        }

        get => _speed;
    }

    public float SpeedBonus
    {
        set
        {
            _speedBonus = Mathf.Max(0, value);
            GetComponent<PlayerMovement>()._speed = FinalSpeed;
        }
        get => _speedBonus;
    }

    public bool Die
    {
        protected set
        {
            _isDie = value;

            if (_isDie && IsOwner)
            {
                UIManager.Instance.OnRespawn();
            }
        }
        get => _isDie;
    }

    public float AttackRange
    {
        set => _attackRange = Mathf.Max(0, value);
        get => _attackRange;
    }

    #endregion

    #region 플레이어 최종 스탯
    public float FinalHp => _maxHp.Value * (1 + (_hpBonus * 0.01f));
    public float FinalHpRegen => _hpRegen * (1 + (_hpRegenBonus * 0.01f));
    public float FinalMp => _maxMp.Value * (1 + (_mpBonus * 0.01f));
    public float FinalMpRegen => _mpRegen * (1 + (_mpRegenBonus * 0.01f));
    public float FinalAttack => _attack * (1 + (_attackBonus * 0.01f));
    public float FinalAS => _attackSpeed * (1 + (_asBonus * 0.01f));
    public float FinalDefense => _defense * (1 + (_defenseBonus * 0.01f));
    public float FinalSpeed => _speed * (1 + (_speedBonus * 0.01f));
    #endregion

    #region 플레이어 이벤트 처리
    public void Hit(float damage, bool isCritical)
    {
        if (!IsOwner) return;

        if (Die)
            return;

        float finalDamage = damage - FinalDefense;

        if (finalDamage <= 0)
            finalDamage = 1;

        Hp -= finalDamage;

        ShowFloatingDamageServerRpc(finalDamage, isCritical);

        _audio.PlayHitSFX();

        if (Hp == 0f)
        {
            OnDie();
        }
    }

    IEnumerator HitEffect()
    {
        if (!IsOwner) yield break;

        SPUM_SpriteList spumList = transform.GetChild(0).GetComponent<SPUM_SpriteList>();
        if (spumList == null)
            yield break;

        List<SpriteRenderer> itemList = spumList._itemList;
        List<SpriteRenderer> armorList = spumList._armorList;
        List<SpriteRenderer> bodyList = spumList._bodyList;

        // 캐릭터의 Hair 색은 변경하지않음
        var filterItemList = itemList.Skip(2).ToList();

        foreach (var item in filterItemList)
        {
            item.color = Color.red;
        }

        foreach (var armor in armorList)
        {
            armor.color = Color.red;
        }

        foreach (var body in bodyList)
        {
            body.color = Color.red;
        }

        yield return new WaitForSeconds(0.2f);

        foreach (var item in filterItemList)
        {
            item.color = Color.white;
        }

        foreach (var armor in armorList)
        {
            armor.color = Color.white;
        }

        foreach (var body in bodyList)
        {
            body.color = Color.white;
        }
    }

    protected void OnDie()
    {
        if (!IsOwner) return;

        Die = true;
        Hp = 0f;
        _target = null;
        _playerRig.velocity = Vector2.zero;

        _animator.SetTrigger("Die");
        // 사망시 이동 입력, 충돌, 공격 정지
        DiePlayerServerRpc();
        this.GetComponent<PlayerMovement>().enabled = false;
        this.GetComponent<PlayerFindTarget>().enabled = false;
        _audio.PlayDeathSFX();

        // 체력, 마나 재생 정지 & 리스폰 기능 활성화
        StopCoroutine("Regen");
        StartCoroutine("Respawn");
    }

    [ServerRpc(RequireOwnership = false)]
    private void DiePlayerServerRpc()
    {
        DiePlayerClientRpc();
    }
    [ClientRpc]
    private void DiePlayerClientRpc()
    {
        this.GetComponent<Collider2D>().enabled = false;
        this.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }

    IEnumerator Respawn()
    {
        // 플레이어 Respawn
        if (!IsOwner) yield break;

        yield return new WaitForSeconds(5f);
        ActiveFalseServerRpc();

        yield return new WaitForSeconds(5f);
        this.transform.position = _spawnPoint.transform.position + new Vector3(0f, 1f, 0f);
        RespawnPlayerServerRpc();
        this.GetComponent<PlayerFindTarget>().enabled = true;
        this.GetComponent<PlayerMovement>().enabled = true;

        _isDie = false;
        Hp = FinalHp;
        Mp = FinalMp;

        StartCoroutine("Regen");


        _animator.SetTrigger("Respawn");
    }

    [ServerRpc(RequireOwnership = false)]
    private void RespawnPlayerServerRpc()
    {
        RespawnPlayerClientRpc();
    }
    [ClientRpc]
    private void RespawnPlayerClientRpc()
    {
        this.transform.GetChild(0).gameObject.SetActive(true);
        this.transform.GetChild(1).gameObject.SetActive(true);
        this.GetComponent<Collider2D>().enabled = true;
        this.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void ActiveFalseServerRpc()
    {
        ActiveFalseClientRpc();
    }

    [ClientRpc]
    private void ActiveFalseClientRpc()
    {
        this.transform.GetChild(0).gameObject.SetActive(false);
        this.transform.GetChild(1).gameObject.SetActive(false);
    }

    [ServerRpc(RequireOwnership = false)]
    public void AttackPlayerServerRpc(float damage, bool isCritical, ServerRpcParams param = default)
    {
        foreach (var client in NetworkManager.Singleton.ConnectedClients)
        {
            if (param.Receive.SenderClientId != client.Key)
            {
                client.Value.PlayerObject.GetComponent<Player>().AttackPlayerClientRpc(client.Key, damage, isCritical);
            }
        }
    }

    [ClientRpc]
    public void AttackPlayerClientRpc(ulong clientId, float damage, bool isCritical)
    {
        // 공격 받은 클라이언트라면 Hit() 처리
        if (clientId == NetworkManager.Singleton.LocalClientId)
            GameManager.Instance.player.Hit(damage: damage, isCritical);
    }

    [ServerRpc(RequireOwnership = false)]
    public void ShowFloatingDamageServerRpc(float damage, bool isCritical)
    {
        ShowFloatingDamageClientRpc(damage, isCritical);
    }

    [ClientRpc]
    public void ShowFloatingDamageClientRpc(float damage, bool isCritical)
    {
        // 피격데미지 표시
        var dmg = Instantiate(_floatingDamage, transform.position + new Vector3(0f, 1f, 0f), Quaternion.identity);

        if (isCritical)
            dmg.GetComponent<TextMesh>().color = Color.red;

        dmg.GetComponent<TextMesh>().text = $"-" + damage.ToString("F1");
    }

    virtual protected void LevelUp()
    {
        if (!IsOwner) return;

        _exp -= _nextExp;
        NextExp *= 1.5f;

        Level += 1;
        LvPoint += 5;

        UIManager.Instance.ExpChanged();

        if (Level == 5)
        {
            Destroy(UIManager.Instance.locked[0]);
        }

        else if (Level == 10)
        {
            Destroy(UIManager.Instance.locked[1]);
        }

        if (_exp >= _nextExp)
        {
            LevelUp();
        }
    }

    IEnumerator Regen()
    {
        if (!IsOwner) yield break;

        if (!_isDie)
        {
            if (Hp < FinalHp)
            {
                Hp += FinalHpRegen;

                if (Hp >= FinalHp)
                    Hp = FinalHp;
            }

            if (Mp < FinalMp)
            {
                Mp += FinalMpRegen;

                if (Mp >= FinalMp)
                    Mp = FinalMp;
            }
        }

        yield return WaitForSecondsCache.Wait(1);

        StartCoroutine("Regen");
    }
    public bool DieCheck()
    {
        return Die;
    }
    #endregion

    #region 테스트용 함수
    private void Update()
    {
        if (!IsOwner) return;

        if (Input.GetKeyDown(KeyCode.G))
        {
            GetGold();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            GetExp();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            GetStrong();
        }
    }

    [ContextMenu("Get Gold")]
    virtual public void GetGold()
    {
        Gold += 500000;
    }

    [ContextMenu("Get Exp")]
    virtual public void GetExp()
    {
        Exp += 1000;
    }

    private void GetStrong()
    {
        Attack += 800f;
        Defense += 500f;
        AttackSpeed += 2f;
        MaxHp += 1000f;
        MaxMp += 1000f;
        HpRegen += 100f;
        MpRegen += 100f;
        Speed += 3f;
    }

    #endregion
}
