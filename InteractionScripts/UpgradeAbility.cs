using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeAbility : MonoBehaviour
{
    [Serializable]
    public struct HUD // UI 변수
    {
        public Button btn;
        public Text level;
        public Text value;
        public Text cost;
    }

    [Serializable]
    public struct UpgradeInfo   // 업그레이드 정보 
    {
        public enum upgradeType { Attack, AttackSpeed, Critical, MaxHp, HpRegen, Defense, MaxMp, MpRegen };
        public upgradeType type;
        public float incValue;
        public float incCost;
        public int level;
        public int maxLevel;
    }
    float _myValue;

    public HUD myUI;
    public UpgradeInfo upgradeInfo;

    private Player _player;

    private void Awake()
    {
        // 플레이어 찾기
        _player = GameManager.Instance.player;

        // 버튼 클릭시 실행할 함수 연결
        myUI.btn.onClick.AddListener(Upgrade);

        // 업그레이드 레벨
        upgradeInfo.level = 0;

        // 초기값 세팅
        switch (upgradeInfo.type)
        {
            case UpgradeInfo.upgradeType.Attack:
                _myValue = _player.Attack;
                break;

            case UpgradeInfo.upgradeType.AttackSpeed:
                _myValue = _player.AttackSpeed;
                break;

            case UpgradeInfo.upgradeType.Critical:
                _myValue = _player.Critical;
                break;

            case UpgradeInfo.upgradeType.MaxHp:
                _myValue = _player.MaxHp;
                break;

            case UpgradeInfo.upgradeType.HpRegen:
                _myValue = _player.HpRegen;
                break;

            case UpgradeInfo.upgradeType.Defense:
                _myValue = _player.Defense;
                break;

            case UpgradeInfo.upgradeType.MaxMp:
                _myValue = _player.MaxMp;
                break;

            case UpgradeInfo.upgradeType.MpRegen:
                _myValue = _player.MpRegen;
                break;
        }
    }

    private void Start()
    {
        // UI 초기화
        InitUI();
    }

    private void InitUI()
    {
        // UI 초기화
        switch (upgradeInfo.type)
        {
            case UpgradeInfo.upgradeType.Attack:
                SetUI(myUI.level, myUI.value, myUI.cost, upgradeInfo.level, _myValue, upgradeInfo.incValue, 0, "공격력");
                break;

            case UpgradeInfo.upgradeType.AttackSpeed:
                SetUI(myUI.level, myUI.value, myUI.cost, upgradeInfo.level, _myValue, upgradeInfo.incValue, 0, "공격속도");
                break;

            case UpgradeInfo.upgradeType.Critical:
                SetUI(myUI.level, myUI.value, myUI.cost, upgradeInfo.level, _myValue, upgradeInfo.incValue, 0, "크리티컬 확률");
                break;

            case UpgradeInfo.upgradeType.MaxHp:
                SetUI(myUI.level, myUI.value, myUI.cost, upgradeInfo.level, _myValue, upgradeInfo.incValue, 0, "체력");
                break;

            case UpgradeInfo.upgradeType.HpRegen:
                SetUI(myUI.level, myUI.value, myUI.cost, upgradeInfo.level, _myValue, upgradeInfo.incValue, 0, "체력 재생속도");
                break;

            case UpgradeInfo.upgradeType.Defense:
                SetUI(myUI.level, myUI.value, myUI.cost, upgradeInfo.level, _myValue, upgradeInfo.incValue, 0, "방어력");
                break;

            case UpgradeInfo.upgradeType.MaxMp:
                SetUI(myUI.level, myUI.value, myUI.cost, upgradeInfo.level, _myValue, upgradeInfo.incValue, 0, "마나");
                break;

            case UpgradeInfo.upgradeType.MpRegen:
                SetUI(myUI.level, myUI.value, myUI.cost, upgradeInfo.level, _myValue, upgradeInfo.incValue, 0, "마나 재생속도");
                break;
        }
    }

    private void Upgrade()
    {
        
        
        // 골드가 충분하면 업그레이드 진행
        if (_player.Gold >= Int32.Parse(myUI.cost.text) && upgradeInfo.level < upgradeInfo.maxLevel)
        {
            _player.Gold -= Int32.Parse(myUI.cost.text);
            UISoundManager.Instance.PlayClickSound();
        }

        else 
        {
            UISoundManager.Instance.PlayCantBuySound();
            return;
        }


        _myValue += upgradeInfo.incValue;
        upgradeInfo.level += 1;
        
        switch (upgradeInfo.type)
        {
            case UpgradeInfo.upgradeType.Attack:
                _player.Attack += upgradeInfo.incValue;
                SetUI(myUI.level, myUI.value, myUI.cost, upgradeInfo.level, _myValue, upgradeInfo.incValue, upgradeInfo.incCost, "공격력");
                break;

            case UpgradeInfo.upgradeType.AttackSpeed:
                _player.AttackSpeed += upgradeInfo.incValue;
                SetUI(myUI.level, myUI.value, myUI.cost, upgradeInfo.level, _myValue, upgradeInfo.incValue, upgradeInfo.incCost, "공격속도");
                break;

            case UpgradeInfo.upgradeType.Critical:
                _player.Critical += upgradeInfo.incValue;
                SetUI(myUI.level, myUI.value, myUI.cost, upgradeInfo.level, _myValue, upgradeInfo.incValue, upgradeInfo.incCost, "크리티컬 확률");
                break;

            case UpgradeInfo.upgradeType.MaxHp:
                _player.MaxHp += upgradeInfo.incValue;
                SetUI(myUI.level, myUI.value, myUI.cost, upgradeInfo.level, _myValue, upgradeInfo.incValue, upgradeInfo.incCost, "체력");
                break;

            case UpgradeInfo.upgradeType.HpRegen:
                _player.HpRegen += upgradeInfo.incValue;
                SetUI(myUI.level, myUI.value, myUI.cost, upgradeInfo.level, _myValue, upgradeInfo.incValue, upgradeInfo.incCost, "체력 재생속도");
                break;

            case UpgradeInfo.upgradeType.Defense:
                _player.Defense += upgradeInfo.incValue;
                SetUI(myUI.level, myUI.value, myUI.cost, upgradeInfo.level, _myValue, upgradeInfo.incValue, upgradeInfo.incCost, "방어력");
                break;

            case UpgradeInfo.upgradeType.MaxMp:
                _player.MaxMp += upgradeInfo.incValue;
                SetUI(myUI.level, myUI.value, myUI.cost, upgradeInfo.level, _myValue, upgradeInfo.incValue, upgradeInfo.incCost, "마나");
                break;

            case UpgradeInfo.upgradeType.MpRegen:
                _player.MpRegen += upgradeInfo.incValue;
                SetUI(myUI.level, myUI.value, myUI.cost, upgradeInfo.level, _myValue, upgradeInfo.incValue, upgradeInfo.incCost, "마나 재생속도");
                break;
        }

        if(upgradeInfo.level >= upgradeInfo.maxLevel)
        {
            myUI.btn.interactable = false;
            myUI.cost.text = "완료";
            

            if(upgradeInfo.type == UpgradeInfo.upgradeType.Critical)
                myUI.value.text = $"{Math.Round(_myValue, 2)}%";

            else if(upgradeInfo.type == UpgradeInfo.upgradeType.HpRegen || upgradeInfo.type == UpgradeInfo.upgradeType.MpRegen)
                myUI.value.text = $"초당 {Math.Round(_myValue, 2)}";
            
            else
                myUI.value.text = $"{Math.Round(_myValue, 2)}";
            return;
        }
    }

    private void SetUI(Text level, Text value, Text cost, int Lv, float initValue, float increase, float costInc, string name)
    {
        //현재 값과 다음에 증가된 값을 UI에 표시
        if (name == "크리티컬 확률")
        {
            level.text = $"Lv{Lv + 1} {name}";
            value.text = $"{initValue}% -> {initValue + increase}%";
            cost.text = $"{Mathf.Round(float.Parse(cost.text) + costInc)}";
            return;
        }

        else if (name == "체력 재생속도" || name == "마나 재생속도")
        {
            level.text = $"Lv{Lv + 1} {name}";
            value.text = $"초당 {Math.Round(initValue, 2)} -> {Math.Round(initValue + increase, 2)}";
            cost.text = $"{Mathf.Round(float.Parse(cost.text) + costInc)}";
            return;
        }

        level.text = $"Lv{Lv + 1} {name}";
        value.text = $"{Math.Round(initValue, 2)} -> {Math.Round(initValue + increase, 2)}";
        cost.text = $"{Mathf.Round(float.Parse(cost.text) + costInc)}";
    }
}
