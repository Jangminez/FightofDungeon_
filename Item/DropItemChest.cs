using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DropItemChest : NetworkBehaviour
{
    [SerializeField] private Button _pickUpBtn;
    public ScriptableItem _item;

    private void Awake()
    {
        _pickUpBtn = UIManager.Instance.pickUpButton;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == 17)
        {
            // 상호작용 버튼 활성화 및 이벤트 추가
            _pickUpBtn.GetComponent<Button>().onClick.AddListener(PickUpItem);
            _pickUpBtn.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.layer == 17)
        {
            // 상호작용 버튼 비활성화 및 이벤트 제거
            _pickUpBtn.GetComponent<Button>().onClick.RemoveAllListeners();
            _pickUpBtn.gameObject.SetActive(false);
        }
    }

    private void PickUpItem()
    {
        if (Inventory.Instance.CheckSlot())
        {
            // 아이템 인벤토리 추가
            Inventory.Instance.PickUpItem(_item);
            DespawnNetworkObjectServerRpc();
        }

        else
        {
            UISoundManager.Instance.PlayCantBuySound();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void DespawnNetworkObjectServerRpc()
    {
        this.GetComponent<NetworkObject>().Despawn();
        Destroy(this.gameObject);
    }
}
