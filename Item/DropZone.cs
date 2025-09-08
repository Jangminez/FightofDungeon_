using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class DropZone : MonoBehaviour, IDropHandler
{
    public enum ZoneType {Drop, Sell};
    public ZoneType _type;
    private Player _player;

    void Awake()
    {
        _player = GameManager.Instance.player;
    }

    public void OnDrop(PointerEventData eventData)
    {
        Equipment dragItem = eventData.pointerDrag.GetComponent<Equipment>();

        if(dragItem != null)
        {
            switch(_type)
            {
                case ZoneType.Drop:
                    dragItem.UnEquipmentItem();
                    DropItem(dragItem._item);
                    Debug.Log("아이템 드랍");

                    UISoundManager.Instance.PlayBuySound();
                    break;

                case ZoneType.Sell:
                    dragItem.SellItem();
                    Debug.Log("아이템 판매");
                    
                    UISoundManager.Instance.PlayBuySound();
                    break;
            }

            UIManager.Instance.isDragItem = false;
        }
    }

    private void DropItem(ScriptableItem item)
    {
        DropItemManager.Instance.DropItemServerRpc(_player.transform.position, item.Id, _player.GetComponent<SortingGroup>().sortingLayerID);
    }
}
