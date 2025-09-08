using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Equipment _equipment;
    private Image _img;
    [HideInInspector] public Transform _preParent;
    void Awake()
    {
        _equipment = GetComponent<Equipment>();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if(_equipment.isComsumable){
            _equipment.UseBtn.gameObject.SetActive(false);
        }
        UIManager.Instance.isDragItem = true;

        _img = GetComponent<Image>();
        
        _preParent = transform.parent;

        // 드래그하는 아이템이 UI 최상단에 보이기 위함
        transform.SetParent(transform.root.GetChild(0));
        transform.SetAsLastSibling();
        _img.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        UISoundManager.Instance.PlayClickSound();
        
        transform.SetParent(_preParent);
        SetButton();

        UIManager.Instance.isDragItem = false;
        _img.raycastTarget = true;
    }

    void SetButton() 
    {
        _equipment._slotBtn.onClick.RemoveAllListeners();
        _equipment._slotBtn = _preParent.GetComponent<Button>();
        _equipment._slotBtn.onClick.AddListener(GetComponent<Equipment>().ClickSlot);
    }
}
