using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public GameObject _upgradeArrow;
    public GameObject _shopArrow;
    public Transform _slot1;
    public Text stepText;
    public Text stepContents;
    public UnityEvent CheckItem;
    bool isFirst = false;

    void Awake()
    {
        _upgradeArrow.SetActive(false);
        _shopArrow.SetActive(false);
    }

    void Update()
    {
        if (_slot1.childCount == 1 && !isFirst)
        {
            isFirst = true;
            CheckItem.Invoke();
        }
    }

    public void UpgradeArrow()
    {
        if (_upgradeArrow.activeSelf)
            _upgradeArrow.SetActive(false);

        else
        {
            stepText.text = "튜토리얼 단계(1/3)";
            stepContents.text = "능력치 업그레이드 확인해보기";
            _upgradeArrow.SetActive(true);
        }
    }

    public void ShopArrow()
    {
        if (_shopArrow.activeSelf)
            _shopArrow.SetActive(false);

        else
        {
            stepText.text = "튜토리얼 단계(2/3)";
            stepContents.text = "상점 확인해보기";
            _shopArrow.SetActive(true);
        }
    }

    public void GiveGold()
    {
        GameManager.Instance.player.Gold += 300;
        _shopArrow.SetActive(false);

        stepText.text = "튜토리얼 단계(3/3)";
        stepContents.text = "상점 아이템 구매해보기";
    }
}
