using UnityEngine;
using UnityEngine.EventSystems;

public class HideUI : MonoBehaviour, IPointerClickHandler
{
    public Transform _information;
    
    public void OnPointerClick(PointerEventData eventData)
    {
        UISoundManager.Instance.PlayExitSound();

        _information.gameObject.SetActive(false);
    }
}
