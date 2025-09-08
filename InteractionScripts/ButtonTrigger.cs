using UnityEngine;
using UnityEngine.Events;

public class ButtonTrigger : MonoBehaviour
{
    bool isActive = false;
    public string objectname;
    public UnityEvent OnClick;

    public void ButtonClick()
    {
        if(objectname == "Shop"){
            GameManager.Instance.player.Gold += 300;
        }
        if(!isActive){
            isActive = true;
            OnClick.Invoke();
        }
        else
            return;
    }
}
