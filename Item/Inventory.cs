using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private static Inventory _instance;

    public List<Transform> _slots;
    public static Inventory Instance
    {
        get
        {
            // 싱글톤 구현
            if (!_instance)
            {
                _instance = FindObjectOfType(typeof(Inventory)) as Inventory;

                if (_instance == null)
                    Debug.Log("인스턴스를 생성합니다");
            }
            return _instance;
        }
    }

    private void Awake()
    {
        // 인스턴스가 없을 때 해당 오브젝트로 설정
        if (_instance == null)
            _instance = this;

        // 인스턴스가 존재한다면 현재 오브젝트 파괴
        else if (_instance != null)
            Destroy(gameObject);
    }

    public void AddInventory(ScriptableItem scriptable) 
    {
        foreach(Transform slot in _slots)
        {
            // 슬롯이 비어있다면 아이템 장착
            if(slot.childCount == 0)
            {
                // 인터페이스 확인 후 장착 & 골드 차감
                Equipment equipment = scriptable.item.GetComponent<Equipment>();
                if(equipment != null)
                {
                    equipment.EquipmentItem();
                    Instantiate(scriptable.item, slot);
                    GameManager.Instance.player.Gold -= Int32.Parse(scriptable.itemCost);
                    break;
                }
            }
            // 슬롯에 아이템이 있다면 다음 슬롯 확인
            else
                continue;
            
        }
    }

    public void PickUpItem(ScriptableItem scriptable)
    {
        foreach(Transform slot in _slots)
        {
            // 슬롯이 비어있다면 아이템 장착
            if(slot.childCount == 0)
            {
                // 인터페이스 확인 후 장착
                Equipment equipment = scriptable.item.GetComponent<Equipment>();
                if(equipment != null)
                {
                    equipment.EquipmentItem();
                    Instantiate(scriptable.item, slot);
                    break;
                }
            }
            // 슬롯에 아이템이 있다면 다음 슬롯 확인
            else
                continue;
            
        }
    }

    public bool CheckSlot()
    {
        foreach(Transform slot in _slots)
        {
            // 슬롯이 비어있다면 아이템 장착
            if(slot.childCount == 0)
            {
                return true;
            }
            // 슬롯에 아이템이 있다면 다음 슬롯 확인
            else
                continue;
            
        }
        return false;
    }
}
