using UnityEngine;
using UnityEngine.UI;

public class SelectCharaterManager : MonoBehaviour
{
    public GameObject isSelect;
    public Button selectButton;
    public GameObject description;
    [Header("Description UI")]
    public Text characterName;
    public Image img;
    public Text hp;
    public Text mp;
    public Text attack;
    public Text attackSpeed;
    public Text defense;
    public Text speed;
    public Text critical;

    private Transform parentTransfrom;
    private ScriptableCharacter selectCharacter;

    void Awake()
    {
        if(selectButton != null)
        {
            selectButton.onClick.AddListener(ClickButton);
        }
    }

    public void SelectCharacter(ScriptableCharacter character, Transform tr)
    {
        selectCharacter = character;

        // UI 요소 설정
        characterName.text = selectCharacter.characterName;
        img.sprite = selectCharacter.characterImg;
        hp.text = selectCharacter.hp.ToString("F1");
        mp.text = selectCharacter.mp.ToString("F1");
        attack.text = selectCharacter.attack.ToString("F1");
        attackSpeed.text = selectCharacter.attackSpeed.ToString("F1");
        defense.text = selectCharacter.defense.ToString("F1");
        speed.text = selectCharacter.speed.ToString("F1");
        critical.text = selectCharacter.critical.ToString("F1");

        description.SetActive(true);

        parentTransfrom = tr;
    }

    public void ClickButton()
    {
        GameManager.Instance.playerPrefabName = selectCharacter.characterName;
        isSelect.transform.SetParent(parentTransfrom);
        isSelect.transform.localPosition = Vector3.zero;
        description.SetActive(false);
    }
}
