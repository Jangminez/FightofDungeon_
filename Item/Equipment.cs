using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Equipment : MonoBehaviour
{
    public bool isComsumable = false;
    public ScriptableItem _item;
    [HideInInspector] public Button _slotBtn;
    [SerializeField] private Button _useBtn;
    public Button UseBtn => _useBtn;
    private GameObject selectOb;
    void Awake()
    {
        _slotBtn = this.transform.parent.GetComponent<Button>();
        if(_slotBtn != null){
            _slotBtn.onClick.AddListener(ClickSlot);   
        }

        // 소비 아이템이라면 버튼에 사용 이벤트 추가
        if(isComsumable){
            _useBtn = this.transform.GetChild(0).GetComponent<Button>();
            _useBtn.onClick.AddListener(UseItem);
        }
    }
    void FixedUpdate()
    {
        //다른 버튼 클릭 시 사용 버튼 비활성화
        selectOb = EventSystem.current.currentSelectedGameObject;

        if(isComsumable &&_useBtn.gameObject.activeSelf && selectOb != _slotBtn.gameObject && selectOb != _useBtn.gameObject ){
            _useBtn.gameObject.SetActive(false);
       }
    }

    // 장비 장착 시
    public void EquipmentItem()
    {
        // 소모형 아이템이면 리턴
        if(isComsumable)
            return;

        // 아이템 장착
        foreach(var stat in _item.stats)
        {
            ApplyStat(stat.Key.Item1,stat.Key.Item2, stat.Value, true);
        }
    }

    // 장비 해제 시
    public void UnEquipmentItem()
    {
        if(!isComsumable){
        // 아이템 해제 후 UI 인벤토리 슬롯에 존재하는 오브젝트 파괴
            foreach(var stat in _item.stats)
            {
                ApplyStat(stat.Key.Item1,stat.Key.Item2, stat.Value, false);
            }
        }

        Destroy(gameObject);
    }

    // 스탯 적용 값의 타입 & 계산 타입 & 착용 여부 확인 후 적용
    private void ApplyStat(ScriptableItem.ValueType valueType, ScriptableItem.CalType calType, float value, bool isEquip)
    {
        // 장착 시 +, 해제 시 - 로 계산
        int multiplier = isEquip ? 1 : -1; 

        // 각 타입 확인 후 플레이어에게 적용
        if (calType == ScriptableItem.CalType.Plus)
        {
            switch (valueType)
            {
                case ScriptableItem.ValueType.Attack:
                    GameManager.Instance.player.Attack += multiplier * value;
                    break;
                case ScriptableItem.ValueType.AttackSpeed:
                    GameManager.Instance.player.AttackSpeed += multiplier * value;
                    break;
                case ScriptableItem.ValueType.Critical:
                    GameManager.Instance.player.Critical += multiplier * value;
                    break;
                case ScriptableItem.ValueType.Defense:
                    GameManager.Instance.player.Defense += multiplier * value;
                    break;
                case ScriptableItem.ValueType.Hp:
                    GameManager.Instance.player.MaxHp += multiplier * value;
                    break;
                case ScriptableItem.ValueType.HpRegen:
                    GameManager.Instance.player.HpRegen += multiplier * value;
                    break;
                case ScriptableItem.ValueType.Mp:
                    GameManager.Instance.player.MaxMp += multiplier * value;
                    break;
                case ScriptableItem.ValueType.MpRegen:
                    GameManager.Instance.player.MpRegen += multiplier * value;
                    break;
                case ScriptableItem.ValueType.Speed:
                    GameManager.Instance.player.Speed += multiplier * value;
                    break;
            }
        }
        else if (calType == ScriptableItem.CalType.Percentage)
        {
            switch (valueType)
            {
                case ScriptableItem.ValueType.Attack:
                    GameManager.Instance.player.AttackBonus += multiplier * value;
                    break;
                case ScriptableItem.ValueType.AttackSpeed:
                    GameManager.Instance.player.AsBonus += multiplier * value;
                    break;
                case ScriptableItem.ValueType.Critical:
                    Debug.Log("Wrong Setting!!");
                    break;
                case ScriptableItem.ValueType.Defense:
                    GameManager.Instance.player.DefenseBonus += multiplier * value;
                    break;
                case ScriptableItem.ValueType.Hp:
                    GameManager.Instance.player.HpBonus += multiplier * value;
                    break;
                case ScriptableItem.ValueType.HpRegen:
                    GameManager.Instance.player.HpRegenBonus += multiplier * value;
                    break;
                case ScriptableItem.ValueType.Mp:
                    GameManager.Instance.player.MpBonus += multiplier * value;
                    break;
                case ScriptableItem.ValueType.MpRegen:
                    GameManager.Instance.player.MpRegenBonus += multiplier * value;
                    break;
                case ScriptableItem.ValueType.Speed:
                    GameManager.Instance.player.SpeedBonus += multiplier * value;
                    break;
            }
        }
    }

    // 슬롯 버튼 클릭시
    public void ClickSlot()
    {
        UISoundManager.Instance.PlayClickSound();

        if(isComsumable)
        {
            _useBtn.gameObject.SetActive(true);
            EventSystem.current.SetSelectedGameObject(_useBtn.gameObject);
        }
    }

    public void SellItem() 
    {
        UnEquipmentItem();
        // 구매 가격의 80% 반환
        GameManager.Instance.player.Gold += Mathf.RoundToInt(Int32.Parse(_item.itemCost) * 0.8f);
    }

    void UseItem()
    {
        UISoundManager.Instance.PlayClickSound();
        GetComponent<IConsumable>().UseItem();
        UnEquipmentItem();
    }
}
