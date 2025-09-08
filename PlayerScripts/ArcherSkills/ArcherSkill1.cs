using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherSkill1 : Skill
{
    [System.Serializable]
    struct SkillInfo
    {
        public float attackUp; // 공격력 증가 배율
        public float asUp; // 공격속도 증가 배울
        public float criUp; // 크리티컬 확률 증가율
        public float coolDown; // 쿨타임
        public float duration; // 스킬 지속시간
    }

    private float timer;

    [SerializeField]SkillInfo _info;
    void Awake()
    {
        // 스킬 정보 초기화
        _isCoolDown = false;
        _info.attackUp = 30f;
        _info.asUp = 50f;
        _info.criUp = 10;
        _info.coolDown = 30f;
        _info.duration = 20f;
        useMp = 5f;
    }

    public override IEnumerator SkillProcess()
    {
        if(!IsOwner) yield break;
        
        timer = 0f;

        GameManager.Instance.player._audio.PlaySkill1SFX();
        
        // 쿨타임 시작
        StartCoroutine(CoolDown(_info.coolDown));
        
        // 각 플레이어 증가수치 증가 후 지속 시간이 끝나면 다시 효과 제거
        GameManager.Instance.player.AttackBonus += _info.attackUp;
        GameManager.Instance.player.AsBonus += _info.asUp;
        GameManager.Instance.player.Critical += _info.criUp;

        while(timer <= _info.duration && !GameManager.Instance.player.Die)
        {
            timer += 0.5f;
            yield return new WaitForSeconds(0.5f);
        }
        
        SkillOff();
    }

    private void SkillOff()
    {
        _anims[1].SetTrigger("End");

        GameManager.Instance.player.AttackBonus -= _info.attackUp;
        GameManager.Instance.player.AsBonus -= _info.asUp;
        GameManager.Instance.player.Critical -= _info.criUp;
    }
}
