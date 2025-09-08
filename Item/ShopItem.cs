using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    public ScriptableItem _myItem;
    public Transform _information;
    private Button _myBtn;
    [SerializeField]private Button _buyBtn;


    void Awake()
    {
        _myBtn = GetComponent<Button>();
        _myBtn.onClick.AddListener(ClickItem);
        _buyBtn = _information.GetChild(4).GetComponent<Button>();
    }
    void ClickItem() 
    {
        UISoundManager.Instance.PlayClickSound();
        // 아이템 정보 UI에 표시
        SetUI();

        // 기존 리스너 제거 후 새로 할당
        _buyBtn.onClick.RemoveAllListeners();
        _buyBtn.onClick.AddListener(ClickBuyButton);
    }

    void SetUI() 
    {
        _information.gameObject.SetActive(true);
        
        // 아이템 이름, 설명, 가격 UI
        _information.GetChild(0).GetComponent<Text>().text = _myItem.itemName;
        _information.GetChild(1).GetComponent<Text>().text = _myItem.itemDescription;
        _information.GetChild(3).GetComponent<Text>().text = _myItem.itemCost;

        // 조합에 필요한 아이템 UI
        if(_myItem.needItem.Count != 0){
            List<string> itemNames = new List<string>();

            foreach(var item in _myItem.needItem){
                itemNames.Add(item.itemName);
            }

            _information.GetChild(5).GetComponent<Text>().text = string.Join(" + ", itemNames);
        }

        else{
            _information.GetChild(5).GetComponent<Text>().text = "";
        }

        // 아이템 적용 능력치 UI
        List<string> itemStats = new List<string>();

        foreach(var stat in _myItem.stats)
        {
            itemStats.Add(StatText(stat.Key.Item1, stat.Key.Item2, stat.Value));
        }

        _information.GetChild(2).GetComponent<Text>().text = string.Join("\n", itemStats);
    }
    
    // 아이템 효과 UI 설정
    string StatText(ScriptableItem.ValueType valueType, ScriptableItem.CalType calType, float value)
    {
        string statText = "";

            switch (valueType)
            {
                case ScriptableItem.ValueType.Attack:
                statText = $"공격력 +{value}";
                break;

                case ScriptableItem.ValueType.AttackSpeed:
                statText = $"공격속도 +{value}";
                break;

                case ScriptableItem.ValueType.Critical:
                statText = $"크리티컬 확률 +{value}%";
                break;

                case ScriptableItem.ValueType.Defense:
                statText = $"방어력 +{value}";
                break;

                case ScriptableItem.ValueType.Hp:
                statText = $"체력 +{value}";
                break;

                case ScriptableItem.ValueType.HpRegen:
                statText = $"체력 재생 +{value}";
                break;

                case ScriptableItem.ValueType.Mp:
                statText = $"마나 +{value}";
                break;

                case ScriptableItem.ValueType.MpRegen:
                statText = $"마나 재생 +{value}";
                break;

                case ScriptableItem.ValueType.Speed:
                statText = $"이동 속도 +{value}";
                break;
            }

    if (calType == ScriptableItem.CalType.Percentage)
    {
        statText += "%";
    }

    return statText;
    }

    void ClickBuyButton()
    {
        StartCoroutine(BuyItem());
    }

    IEnumerator BuyItem()
    {
        UISoundManager.Instance.PlayClickSound();
        
        // 플레이어의 골드가 충분하다면 구매
        if(GameManager.Instance.player.Gold >= Int32.Parse(_myItem.itemCost) && _myItem.needItem.Count == 0)
        {
            UISoundManager.Instance.PlayBuySound();

            Inventory.Instance.AddInventory(_myItem);
        }

        else if(GameManager.Instance.player.Gold >= Int32.Parse(_myItem.itemCost) && _myItem.needItem.Count > 0)
        {
            // 인벤토리에 제작에 필요한 아이템이 있다면 담을 변수 생성
            List<Transform> items;
            items = new List<Transform>();

            // 필요 아이템 반복
            foreach(var item in _myItem.needItem)
            {
                // 해당 아이템이 인벤토리 슬롯에 존재하는지 확인
                foreach(var slot in Inventory.Instance._slots)
                {
                    // 존재한다면 위 변수에 추가 후 탐색 할 아이템 변경
                    if(slot.childCount > 0 && item == slot.GetChild(0).GetComponent<Equipment>()._item){
                        items.Add(slot.GetChild(0)); 
                        break; 
                    }
                }
            }
            // 아이템이 모두 존재한다면 해당 아이템 슬롯에서 제거 & 구매 아이템 추가
            if(items.Count == _myItem.needItem.Count){
                foreach (var item in items){
                    item.GetComponent<Equipment>().UnEquipmentItem();
                }
                // 한 프레임 쉬고 아이템 추가 장비 사라짐 방지
                yield return null;
                Inventory.Instance.AddInventory(_myItem);
            }

            UISoundManager.Instance.PlayBuySound();
        }

        else
        {
            UISoundManager.Instance.PlayCantBuySound();
        }
    }
}
