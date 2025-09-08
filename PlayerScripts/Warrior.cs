using System.Collections;
using UnityEngine;

public class Warrior : Player
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

        MaxHp = 50.0f;
        Hp = MaxHp;
        HpRegen = 1f;

        MaxMp = 20.0f;
        Mp = MaxMp;
        MpRegen = 0.2f;

        Speed = 3.0f;

        Gold = 0;

        Attack = 6f;
        AttackSpeed = 1.0f;
        Critical = 0.0f;
        AttackRange = 2f;

        Defense = 5.0f;

        Level = 1;
        Exp = 0;
        NextExp = 100;

        StartCoroutine("Regen");
    }
}
