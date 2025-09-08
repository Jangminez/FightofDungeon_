using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Item")]
public class ScriptableItem : ScriptableObject
{
    public GameObject item;
    public string itemName;
    public int Id;
    public enum ValueType {Attack, AttackSpeed, Critical, Defense, Hp, HpRegen, Mp, MpRegen, Speed}
    public enum CalType {Plus, Percentage}
    public List<Stat> statsList = new List<Stat>();
    [TextArea(3, 5)]
    public string itemDescription;
    public string itemCost;
    public List<ScriptableItem> needItem;

    // 유니티는 인스펙터 창에 Dictionary 직렬화 불가능
    [NonSerialized]
    public Dictionary<Tuple<ValueType, CalType>, float> stats;

    [Serializable]
    public struct Stat
    {
        public ValueType valuetype;
        public CalType caltype;
        public float value;
    }

    // ScriptableObject를 사용할 때 스탯 초기화
    void OnEnable()
    {
        InitializeStats();
    }

    private void InitializeStats()
    {
        // Tuple을 키값으로 가지는 Dictionary 생성
        stats = new Dictionary<Tuple<ValueType, CalType>, float>();

        // statsList 안을 반복하면서 key값과 value값 stats Dictionary에 할당
        foreach(Stat stat in statsList)
        {
            var key = new Tuple<ValueType, CalType>(stat.valuetype, stat.caltype);

            if(!stats.ContainsKey(key))
            {
                stats.Add(key, stat.value);
            }
        }
    }

}

