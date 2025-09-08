using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Scriptable Objects/Relic")]
public class ScriptableRelic : ScriptableObject
{
    public short r_Id;
    public MyRelic myRelic;
    public Sprite r_Icon;
    public string r_Name;
    public int r_Level;
    public int r_Count;
    public int r_UpgradeCount;
    public string r_Description;
    public int r_UpgradeCost;
    public int r_UpgradeValue;
    public bool isDraw;

    public enum ValueType {Attack, AttackSpeed, Critical, Defense, Hp, HpRegen, Mp, MpRegen, Speed}
    public enum CalType {Plus, Percentage}

    [Serializable]
    public struct Stat
    {
        public ValueType valuetype;
        public CalType caltype;
        public float value;
    }

    public Stat stat;

}
