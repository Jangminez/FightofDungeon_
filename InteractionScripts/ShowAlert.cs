using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowAlert : MonoBehaviour
{
    public Button _myBtn;
    public GameObject _message;

    void Awake()
    {
        _myBtn = GetComponent<Button>();
        _myBtn.onClick.AddListener(ShowMessage);
    }

    void ShowMessage() 
    {
        UISoundManager.Instance.PlayPopUpSound();
        
        _message.SetActive(true);
    }
}
