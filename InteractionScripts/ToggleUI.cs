using UnityEngine;
using UnityEngine.UI;

public class ToggleUI : MonoBehaviour
{
    [SerializeField]
    private GameObject _toggleUI;
    private bool _isOn;
    private Button _toggleButton;

    void Start()
    {
        _isOn = false;
        _toggleButton = GetComponent<Button>();
        _toggleButton.onClick.AddListener(ToggleObject);
    }

    private void ToggleObject()
    {
        UISoundManager.Instance.PlayClickSound();
        
        // 같은 버튼을 눌렀을 때 UI가 켜지고 꺼짐
        if(!_isOn)
        {
            _toggleUI.SetActive(true);
            _isOn = true;
        }

        else
        {
            _toggleUI.SetActive(false);
            _isOn = false;
        }
    }
}
