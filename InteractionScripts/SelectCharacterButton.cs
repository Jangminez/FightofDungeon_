using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SelectCharacterButton : MonoBehaviour
{
    private Button myBtn;
    public SelectCharaterManager manager;
    public ScriptableCharacter character;
    void Awake()
    {
        myBtn = GetComponent<Button>();
        if(myBtn != null)
        {
            myBtn.onClick.AddListener(ClickButton);
        }
    }

    public void ClickButton()
    {
        UISoundManager.Instance.PlayClickSound();
        
        manager.SelectCharacter(character, transform);
    }

}
