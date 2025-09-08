using System;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpgrade : MonoBehaviour
{
    [SerializeField] float _myValue;

    [Serializable]
    public struct HUD // UI 변수
    {
        public Button btn;
        public Text level;
        public Text value;
        public Button reset;
    }

    [Serializable]
    public struct UpgradeInfo   // 업그레이드 정보 
    {
        public enum upgradeType { Attack, AttackSpeed, Critical, MaxHp, HpRegen, Defense, MaxMp, MpRegen };
        public upgradeType type;
        public float incValue;
        public int level;
        public int maxLevel;
    }

    public HUD myUI;
    public UpgradeInfo upgradeInfo;

    private Player _player;

    private void Awake()
    {
        // 플레이어 찾기
        _player = GameManager.Instance.player;

        // 버튼 클릭시 실행할 함수 연결
        myUI.btn.onClick.AddListener(Upgrade);
        myUI.reset.onClick.AddListener(ResetStat);

        // 업그레이드 레벨
        upgradeInfo.level = 0;
        _myValue = 0f;
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
                SetUI(myUI.level, myUI.value, upgradeInfo.level, _myValue, upgradeInfo.incValue, "공격력");
                break;

            case UpgradeInfo.upgradeType.AttackSpeed:
                SetUI(myUI.level, myUI.value, upgradeInfo.level, _myValue, upgradeInfo.incValue, "공격속도");
                break;

            case UpgradeInfo.upgradeType.Critical:
                SetUI(myUI.level, myUI.value, upgradeInfo.level, _myValue, upgradeInfo.incValue, "크리티컬 확률");
                break;

            case UpgradeInfo.upgradeType.MaxHp:
                SetUI(myUI.level, myUI.value, upgradeInfo.level, _myValue, upgradeInfo.incValue, "체력");
                break;

            case UpgradeInfo.upgradeType.HpRegen:
                SetUI(myUI.level, myUI.value, upgradeInfo.level, _myValue, upgradeInfo.incValue, "체력 재생속도");
                break;

            case UpgradeInfo.upgradeType.Defense:
                SetUI(myUI.level, myUI.value, upgradeInfo.level, _myValue, upgradeInfo.incValue, "방어력");
                break;

            case UpgradeInfo.upgradeType.MaxMp:
                SetUI(myUI.level, myUI.value, upgradeInfo.level, _myValue, upgradeInfo.incValue, "마나");
                break;

            case UpgradeInfo.upgradeType.MpRegen:
                SetUI(myUI.level, myUI.value, upgradeInfo.level, _myValue, upgradeInfo.incValue, "마나 재생속도");
                break;
        }
    }

    private void Upgrade()
    {
        UISoundManager.Instance.PlayClickSound();

        // 레벨 포인트가 있다면 업그레이드 진행
        if (_player.LvPoint > 0 && upgradeInfo.level < upgradeInfo.maxLevel)
        {
            _player.LvPoint -= 1;
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
                SetUI(myUI.level, myUI.value, upgradeInfo.level, _myValue, upgradeInfo.incValue, "공격력");
                break;

            case UpgradeInfo.upgradeType.AttackSpeed:
                _player.AttackSpeed += upgradeInfo.incValue;
                SetUI(myUI.level, myUI.value, upgradeInfo.level, _myValue, upgradeInfo.incValue, "공격속도");
                break;

            case UpgradeInfo.upgradeType.Critical:
                _player.Critical += upgradeInfo.incValue;
                SetUI(myUI.level, myUI.value, upgradeInfo.level, _myValue, upgradeInfo.incValue, "크리티컬 확률");
                break;

            case UpgradeInfo.upgradeType.MaxHp:
                _player.MaxHp += upgradeInfo.incValue;
                SetUI(myUI.level, myUI.value, upgradeInfo.level, _myValue, upgradeInfo.incValue, "체력");
                break;

            case UpgradeInfo.upgradeType.HpRegen:
                _player.HpRegen += upgradeInfo.incValue;
                SetUI(myUI.level, myUI.value, upgradeInfo.level, _myValue, upgradeInfo.incValue, "체력 재생속도");
                break;

            case UpgradeInfo.upgradeType.Defense:
                _player.Defense += upgradeInfo.incValue;
                SetUI(myUI.level, myUI.value, upgradeInfo.level, _myValue, upgradeInfo.incValue, "방어력");
                break;

            case UpgradeInfo.upgradeType.MaxMp:
                _player.MaxMp += upgradeInfo.incValue;
                SetUI(myUI.level, myUI.value, upgradeInfo.level, _myValue, upgradeInfo.incValue, "마나");
                break;

            case UpgradeInfo.upgradeType.MpRegen:
                _player.MpRegen += upgradeInfo.incValue;
                SetUI(myUI.level, myUI.value, upgradeInfo.level, _myValue, upgradeInfo.incValue, "마나 재생속도");
                break;
        }

        // 레벨이 최대 레벨이라면 업그레이드 X
        if (upgradeInfo.level >= upgradeInfo.maxLevel)
        {
            myUI.btn.interactable = false;

            if (upgradeInfo.type == UpgradeInfo.upgradeType.Critical)
                myUI.value.text = $"{Math.Round(_myValue, 2)}%";

            else if (upgradeInfo.type == UpgradeInfo.upgradeType.HpRegen || upgradeInfo.type == UpgradeInfo.upgradeType.MpRegen)
                myUI.value.text = $"초당 {Math.Round(_myValue, 2)}";

            else
                myUI.value.text = $"{Math.Round(_myValue, 2)}";

            return;
        }
    }

    private void SetUI(Text level, Text value, int Lv, float initValue, float increase, string name)
    {
        //UI 세팅
        if (name == "크리티컬 확률")
        {
            level.text = $"Lv{Lv + 1} {name}";
            value.text = $"{initValue}% -> {initValue + increase}%";
            return;
        }

        else if (name == "체력 재생속도" || name == "마나 재생속도")
        {
            level.text = $"Lv{Lv + 1} {name}";
            value.text = $"초당 {Math.Round(initValue, 2)} -> {Math.Round(initValue + increase, 2)}";
            return;
        }

        level.text = $"Lv{Lv + 1} {name}";
        value.text = $"{Math.Round(initValue, 2)} -> {Math.Round(initValue + increase, 2)}";
    }

    void ResetStat()
    {
        UISoundManager.Instance.PlayClickSound();

        // 플레이어의 골드가 충분하면 초기화 진행
        if (_player.Gold >= 3000 && upgradeInfo.type == UpgradeInfo.upgradeType.Attack)
        {
            _player.Gold -= 3000;
        }

        else if (_player.Gold < 3000)
        {
            return;
        }

        switch (upgradeInfo.type)
        {
            case UpgradeInfo.upgradeType.Attack:
                _player.Attack -= _myValue;
                ResetUI(myUI.level, myUI.value, 0f, upgradeInfo.incValue, "공격력");
                break;

            case UpgradeInfo.upgradeType.AttackSpeed:
                _player.AttackSpeed -= _myValue;
                ResetUI(myUI.level, myUI.value, 0f, upgradeInfo.incValue, "공격속도");
                break;

            case UpgradeInfo.upgradeType.Critical:
                _player.Critical -= _myValue;
                ResetUI(myUI.level, myUI.value, 0f, upgradeInfo.incValue, "크리티컬 확률");
                break;

            case UpgradeInfo.upgradeType.MaxHp:
                _player.MaxHp -= _myValue;
                ResetUI(myUI.level, myUI.value, 0f, upgradeInfo.incValue, "체력");
                break;

            case UpgradeInfo.upgradeType.HpRegen:
                _player.HpRegen -= _myValue;
                ResetUI(myUI.level, myUI.value, 0f, upgradeInfo.incValue, "체력 재생속도");
                break;

            case UpgradeInfo.upgradeType.Defense:
                _player.Defense -= _myValue;
                ResetUI(myUI.level, myUI.value, 0f, upgradeInfo.incValue, "방어력");
                break;

            case UpgradeInfo.upgradeType.MaxMp:
                _player.MaxMp -= _myValue;
                ResetUI(myUI.level, myUI.value, 0f, upgradeInfo.incValue, "마나");
                break;

            case UpgradeInfo.upgradeType.MpRegen:
                _player.MpRegen -= _myValue;
                ResetUI(myUI.level, myUI.value, 0f, upgradeInfo.incValue, "마나 재생속도");
                break;
        }

        if (upgradeInfo.level > 0)
        {
            _player.LvPoint += upgradeInfo.level;
            _myValue = 0f;
            upgradeInfo.level = 0;
        }

        myUI.btn.interactable = true;
    }

    void ResetUI(Text level, Text value, float initValue, float increase, string name)
    {
        //UI 세팅
        if (name == "크리티컬 확률")
        {
            level.text = $"Lv1 {name}";
            value.text = initValue.ToString() + "%" + " -> " + (initValue + increase).ToString() + "%";
            return;
        }

        else if (name == "체력 재생속도" || name == "마나 재생속도")
        {
            level.text = $"Lv1 {name}"; ;
            value.text = "초당 " + Math.Round(initValue, 2).ToString() + " -> " + Math.Round(initValue + increase, 2).ToString();
            return;
        }

        level.text = $"Lv1 {name}";
        value.text = Math.Round(initValue, 2).ToString() + " -> " + Math.Round(initValue + increase, 2).ToString();
    }
}
