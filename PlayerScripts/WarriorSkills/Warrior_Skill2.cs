using System.Collections;
using UnityEngine;

public class Warrior_Skill2 : Skill
{
    private Player player;
    [System.Serializable]
    struct SkillInfo
    {
        public float damage; // 데미지  
        public float coolDown; // 쿨타임
    }
    [SerializeField] SkillInfo _info;

    void Awake()
    {
        // 스킬 정보 초기화
        _info.damage = 2f;
        _info.coolDown = 10f;
        useMp = 10f;
    }
    public override IEnumerator SkillProcess()
    {
        if(!IsOwner) yield break;
        GameManager.Instance.player._audio.PlaySkill2SFX();
        
        // 쿨타임 시작
        StartCoroutine(CoolDown(_info.coolDown));
        yield return null;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(!IsOwner) return;

        player = GameManager.Instance.player;

        // 몬스터가 스킬 범위에 들어오면 데미지 적용
        var enemy = other.GetComponent<IDamgeable>();

        cri = Random.Range(1f, 101f); // 1 ~ 100 확률 지정

        if (enemy != null)
        {
            if(other.tag == "Player")
            {
                if(cri <= player.Critical)
                    player.AttackPlayerServerRpc(player.FinalAttack * _info.damage * 1.5f, true);

                else
                    player.AttackPlayerServerRpc(player.FinalAttack * _info.damage, false);
            }
            else
            {
                if(cri <= player.Critical)
                    enemy.Hit(player.FinalAttack * _info.damage * 1.5f, true);

                else
                    enemy.Hit(player.FinalAttack * _info.damage, false);
            }
        }
    }
}
