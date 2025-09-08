using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelicManager : MonoBehaviour
{
    private static RelicManager _instance;
    public static RelicManager Instance
    {
        get
        {
            // 싱글톤 구현
            if (!_instance)
            {
                _instance = FindObjectOfType(typeof(RelicManager)) as RelicManager;

                if (_instance == null)
                    Debug.Log("인스턴스를 생성합니다");
            }
            return _instance;
        }
    }
    private Dictionary<int, ScriptableRelic> relicDictionary = new Dictionary<int, ScriptableRelic>();

    void Awake()
    {
        // 인스턴스가 없을 때 해당 오브젝트로 설정
        if (_instance == null)
            _instance = this;

        // 인스턴스가 존재한다면 현재 오브젝트 파괴
        else if (_instance != null)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        LoadAllRelics();
    }

    private void LoadAllRelics()
    {
        relicDictionary.Clear();

        ScriptableRelic[] relics = Resources.LoadAll<ScriptableRelic>("Relics");
        foreach (var relic in relics)
        {
            if (relicDictionary.ContainsKey(relic.r_Id))
            {
                Debug.Log($"이미 존재하는 아이템입니다. {relic.r_Name}");
            }

            else
            {
                relicDictionary.Add(relic.r_Id, relic);
            }
        }
    }

    public ScriptableRelic GetRelic(int id)
    {
        if (relicDictionary.TryGetValue(id, out var relic))
        {
            return relic;
        }

        Debug.Log($"아이디에 해당하는 유물을 찾을 수 없습니다. \n 아이디: {id}");
        return null;
    }

    public void ApplyRelics()
    {
        foreach (var relic in relicDictionary.Values)
        {
            if (relic.isDraw)
            {
                ApplyRelicStat(relic.stat.valuetype, relic.stat.caltype, relic.stat.value, relic.r_UpgradeValue);
                Debug.Log(relic.r_Name + ": 스탯 적용");
            }
        }
    }

    private void ApplyRelicStat(ScriptableRelic.ValueType valueType, ScriptableRelic.CalType calType, float value, float upgradeValue)
    {
        if (calType == ScriptableRelic.CalType.Plus)
        {
            switch (valueType)
            {
                case ScriptableRelic.ValueType.Attack:
                    GameManager.Instance.player.Attack += value + upgradeValue;
                    break;
                case ScriptableRelic.ValueType.AttackSpeed:
                    GameManager.Instance.player.AttackSpeed += value + upgradeValue;
                    break;
                case ScriptableRelic.ValueType.Critical:
                    GameManager.Instance.player.Critical += value + upgradeValue;
                    break;
                case ScriptableRelic.ValueType.Defense:
                    GameManager.Instance.player.Defense += value + upgradeValue;
                    break;
                case ScriptableRelic.ValueType.Hp:
                    GameManager.Instance.player.MaxHp += value + upgradeValue;
                    break;
                case ScriptableRelic.ValueType.HpRegen:
                    GameManager.Instance.player.HpRegen += value + upgradeValue;
                    break;
                case ScriptableRelic.ValueType.Mp:
                    GameManager.Instance.player.MaxMp += value + upgradeValue;
                    break;
                case ScriptableRelic.ValueType.MpRegen:
                    GameManager.Instance.player.MpRegen += value + upgradeValue;
                    break;
                case ScriptableRelic.ValueType.Speed:
                    GameManager.Instance.player.Speed += value + upgradeValue;
                    break;
            }
        }
        else if (calType == ScriptableRelic.CalType.Percentage)
        {
            switch (valueType)
            {
                case ScriptableRelic.ValueType.Attack:
                    GameManager.Instance.player.AttackBonus += value + upgradeValue;
                    break;
                case ScriptableRelic.ValueType.AttackSpeed:
                    GameManager.Instance.player.AsBonus += value + upgradeValue;
                    break;
                case ScriptableRelic.ValueType.Critical:
                    Debug.Log("Wrong Setting!!");
                    break;
                case ScriptableRelic.ValueType.Defense:
                    GameManager.Instance.player.DefenseBonus += value + upgradeValue;
                    break;
                case ScriptableRelic.ValueType.Hp:
                    GameManager.Instance.player.HpBonus += value + upgradeValue;
                    break;
                case ScriptableRelic.ValueType.HpRegen:
                    GameManager.Instance.player.HpRegenBonus += value + upgradeValue;
                    break;
                case ScriptableRelic.ValueType.Mp:
                    GameManager.Instance.player.MpBonus += value + upgradeValue;
                    break;
                case ScriptableRelic.ValueType.MpRegen:
                    GameManager.Instance.player.MpRegenBonus += value + upgradeValue;
                    break;
                case ScriptableRelic.ValueType.Speed:
                    GameManager.Instance.player.SpeedBonus += value + upgradeValue;
                    break;
            }
        }
    }
}
