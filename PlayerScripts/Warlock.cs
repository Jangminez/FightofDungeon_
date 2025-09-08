using UnityEngine;

public class Warlock : Player
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

        MaxHp = 35.0f;
        Hp = MaxHp;
        HpRegen = 0.5f;

        MaxMp = 30.0f;
        Mp = MaxMp;
        MpRegen = 1f;

        Speed = 2.5f;

        Gold = 0;

        Attack = 10f;
        AttackSpeed = 0.5f;
        Critical = 3f;
        AttackRange = 5f;

        Defense = 2.0f;

        Level = 1;
        Exp = 0;
        NextExp = 100;

        StartCoroutine("Regen");
    }
}
