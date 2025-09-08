using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SlideUpUI : MonoBehaviour
{
    void OnEnable()
    {
        UISoundManager.Instance.PlayPopUpSound();
        
        transform.gameObject.SetActive(true);
        
        transform.DOMoveY(transform.position.y + 1200f, 0.5f)
            .SetEase(Ease.OutBack);  // 위로 튕기듯이 이동
        
        transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
    }
}
