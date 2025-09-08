using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Attack : NetworkBehaviour
{
    public enum attackType { BasicAttack, Projectile, Skill }
    public attackType type;
    private Player player;
    float cri;
    bool isAttack = false;
    public float skillDamage;

    void Awake()
    {
        skillDamage = 1f;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!IsOwner) return;

        if (col.GetComponent<IDamgeable>() != null && !isAttack)
        {
            player = GameManager.Instance.player;

            if (type != attackType.Skill)
            {
                isAttack = true;
            }
            
            cri = Random.Range(1f, 101f); // 1 ~ 100 확률 지정

            // cri의 값이 크리티컬 범위 안에 존재한다면 크리티컬 공격
            if (col.tag == "Player")
            {
                if(cri <= player.Critical)
                    player.AttackPlayerServerRpc(player.FinalAttack * skillDamage * 1.5f, true);

                else
                    player.AttackPlayerServerRpc(player.FinalAttack * skillDamage, false);
            }
            else
            {
                if(cri <= player.Critical)
                    col.GetComponent<IDamgeable>().Hit(player.FinalAttack * skillDamage * 1.5f, true);

                else
                    col.GetComponent<IDamgeable>().Hit(player.FinalAttack * skillDamage, false);
            }

            if (type == attackType.Projectile)
            {
                GetComponent<Animator>().SetTrigger("Hit");
                GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                Destroy(this.gameObject, 1f);
            }
        }

        else
        {
            if(col.gameObject.layer == 7)
                this.gameObject.SetActive(false);
        }
    }
}
