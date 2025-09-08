using UnityEngine;

public class Archer : Player
{
    public override void OnNetworkSpawn()
    {
        _playerRig = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();

        if(!IsOwner) return;

        SetCharater();
    }


    protected override void SetCharater()
    {
        if(!IsOwner) return;
        Die = false;

        MaxHp = 40.0f;
        Hp = MaxHp;
        HpRegen = 1f;

        MaxMp = 20.0f;
        Mp = MaxMp;
        MpRegen = 0.2f;

        Speed = 3.5f;

        Gold = 0;

        Attack = 5f;
        AttackSpeed = 1.0f;
        Critical = 5f;
        AttackRange = 7f;

        Defense = 3.0f;

        Level = 1;
        Exp = 0;
        NextExp = 100;

        StartCoroutine("Regen");
    }
}
