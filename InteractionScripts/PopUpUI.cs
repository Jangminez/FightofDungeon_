using DG.Tweening;
using UnityEngine;

public class PopUpUI : MonoBehaviour
{
    void OnEnable()
    {
        UISoundManager.Instance.PlayPopUpSound();
        
        transform.localScale = Vector3.zero;
        transform.DOScale(1.1f, 0.2f).SetEase(Ease.OutBack);
        transform.DOScale(1f, 0.1f).SetDelay(0.2f);
    }
}
