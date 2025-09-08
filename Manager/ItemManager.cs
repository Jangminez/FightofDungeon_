using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    private static ItemManager _instance;
    public static ItemManager Instance
    {
        get
        {
            // 싱글톤 구현
            if (!_instance)
            {
                _instance = FindObjectOfType(typeof(ItemManager)) as ItemManager;

                if (_instance == null)
                    Debug.Log("인스턴스를 생성합니다");
            }
            return _instance;
        }
    }

    private Dictionary<int, ScriptableItem> itemDictionary = new Dictionary<int, ScriptableItem>();

    private void Awake()
    {
        // 인스턴스가 없을 때 해당 오브젝트로 설정
        if (_instance == null)
            _instance = this;

        // 인스턴스가 존재한다면 현재 오브젝트 파괴
        else if (_instance != null)
            Destroy(gameObject);

        // 아이템 정보 불러오기
        LoadAllItems();
    }

    private void LoadAllItems()
    {
        itemDictionary.Clear();

        ScriptableItem[] items = Resources.LoadAll<ScriptableItem>("Items");
        foreach(var item in items)
        {
            if(itemDictionary.ContainsKey(item.Id))
            {
                Debug.Log($"이미 존재하는 아이템입니다. {item.itemName}");
            }

            else
            {
                itemDictionary.Add(item.Id, item);
            }
        }
    }

    public ScriptableItem GetItem(int id)
    {
        if(itemDictionary.TryGetValue(id, out var item))
        {
            return item;
        }

        Debug.Log($"아이디에 해당하는 아이템을 찾을 수 없습니다. \n 아이디: {id}");
        return null;
    }
}
