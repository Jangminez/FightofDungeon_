using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        Transform dropped = eventData.pointerDrag.transform;
        DraggableItem draggable = dropped.GetComponent<DraggableItem>();

        if (transform.childCount == 0)       
            draggable._preParent = transform;
        
        else
        {
            Transform child = transform.GetChild(0);
            child.GetComponent<DraggableItem>()._preParent = draggable._preParent;
            child.SetParent(draggable._preParent);
            draggable._preParent = transform;
        }
    }
}
