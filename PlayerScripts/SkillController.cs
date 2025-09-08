using Unity.Netcode;
using UnityEngine.UI;

public class SkillController : NetworkBehaviour
{
    public Skill[] _skills;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            this.enabled = false;
            return;
        }

        for (int i = 0; i < _skills.Length; i++)
        {
            UIManager.Instance.skillButtons[i].onClick.AddListener(_skills[i].UseSkill);
            UIManager.Instance.skillButtons[i].image.sprite = _skills[i]._icon;
            _skills[i]._CD = UIManager.Instance.skillButtons[i].transform.parent.GetChild(2).GetComponent<Image>();
            
            Text[] texts = UIManager.Instance.Descriptions[i].GetComponentsInChildren<Text>();

            texts[0].text = _skills[i].skillName;
            texts[1].text = _skills[i].skillDesc;
            texts[2].text = _skills[i].skillCD;
            texts[3].text += _skills[i].useMp.ToString();
        }
    }
}
