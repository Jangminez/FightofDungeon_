using UnityEngine;
using UnityEngine.EventSystems;

public class SkillDescription : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    private GameObject _descriptionUI;
    private float _checkHoldTime = 0.5f;
    private bool isHold;
    private float _holdTime;


    void Update()
    {
        // 버튼을 누른 상태라면 일정 시간 지나면 설명UI 활성화
        if(isHold)
        {
            _holdTime += Time.deltaTime;

            if(_holdTime > _checkHoldTime && !_descriptionUI.activeSelf)
            {
                _descriptionUI.SetActive(true);
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // 포인터가 눌렸을 때 
        isHold = true;
        _holdTime = 0f;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // 포인터를 땠을 때
        isHold = false;
        _holdTime = 0f;
        _descriptionUI.SetActive(false);
    }

}
