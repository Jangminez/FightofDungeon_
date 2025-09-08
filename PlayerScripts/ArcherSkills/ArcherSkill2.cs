using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ArcherSkill2 : Skill
{
    public GameObject _attack;
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
        _info.damage = 2f;
        _info.coolDown = 20f;
        useMp = 15f;
    }

    public override IEnumerator SkillProcess()
    {
        if (!IsOwner) yield break;
        player = GameManager.Instance.player;
        player.Mp += useMp;

        // 스킬 이펙트 소환
        if (player._target != null)
        {
            player.Mp -= useMp;
            
            // 쿨타임 시작
            StartCoroutine(CoolDown(_info.coolDown));

            SpawnAttackServerRpc(player._target.position + new Vector3(-0.5f, 1f));
            GameManager.Instance.player._audio.PlaySkill2SFX();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnAttackServerRpc(Vector3 targetPosition, ServerRpcParams rpcParams = default)
    {
        // 서버에서 스킬을 사용한 클라이언트의 소유로 스킬 이펙트 생성 및 삭제
        GameObject attack = Instantiate(_attack, targetPosition, Quaternion.identity);
        attack.GetComponent<NetworkObject>().SpawnWithOwnership(rpcParams.Receive.SenderClientId);
        SetAttackClientRpc(attack.GetComponent<NetworkObject>().NetworkObjectId);
        Destroy(attack, 2.5f);
    }

    [ClientRpc]
    private void SetAttackClientRpc(ulong objectId)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(objectId, out var attackObject))
        {
            if (attackObject.OwnerClientId == NetworkManager.Singleton.LocalClientId)
            {
                // 해당 클라이언트 공격 적용
                Attack attack = attackObject.GetComponent<Attack>();
                attack.skillDamage = _info.damage;
                Destroy(attack, 2.5f);
            }
        }
    }
}
