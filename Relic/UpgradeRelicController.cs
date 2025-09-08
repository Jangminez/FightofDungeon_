using UnityEngine;
using UnityEngine.UI;

public class UpgradeRelicController : MonoBehaviour
{
    public Image r_Icon;
    public Text r_Name;
    public Text r_Level;
    public Text r_Count;
    public Image r_Bar;
    public Text r_Description;
    public Text r_UpgradeValue;
    public Text r_Cost;
    public Button r_UpgradeButton;
    ScriptableRelic selectedRelic;
    public GameObject info;

    void Awake()
    {
        if (r_UpgradeButton != null)
            r_UpgradeButton.onClick.AddListener(UpgradeRelic);
    }

    public void ClickRelic(ScriptableRelic relic)
    {
        selectedRelic = relic;

        // UI 에 유물정보 할당
        r_Icon.sprite = relic.r_Icon;
        r_Name.text = relic.r_Name;
        r_Level.text = $"Lv.{relic.r_Level}";
        r_Count.text = $"{relic.r_Count} / {relic.r_UpgradeCount}";
        r_Bar.fillAmount = (float)relic.r_Count / (float)relic.r_UpgradeCount;
        r_Description.text = relic.r_Description;
        r_UpgradeValue.text = $"(+{relic.r_UpgradeValue})";
        r_Cost.text = relic.r_UpgradeCost.ToString();

        if (selectedRelic.r_Count >= selectedRelic.r_UpgradeCount)
            r_UpgradeButton.interactable = true;
        else
            r_UpgradeButton.interactable = false;

        // 유물 업그레이드 창 활성화
        info.SetActive(true);
    }

    public void UpgradeRelic()
    {
        // 전체 메인 골드 확인하는 부분 필요
        if (GameManager.Instance.Gold >= selectedRelic.r_UpgradeCost)
        {
            GameManager.Instance.Gold -= selectedRelic.r_UpgradeCost;
            selectedRelic.r_Count -= selectedRelic.r_UpgradeCount;
            selectedRelic.r_UpgradeCount += 3;
            selectedRelic.r_Level++;
            selectedRelic.r_UpgradeCost += 1000;
            selectedRelic.r_UpgradeValue += 2;

            // UI 변경값 적용
            ClickRelic(selectedRelic);
            selectedRelic.myRelic.SetUI();

            UISoundManager.Instance.PlayClickSound();

            // 플레이어 데이터 저장
            GameManager.Instance.SavePlayerData();
        }

        else
        {
            UISoundManager.Instance.PlayCantBuySound();
        }
    }
}
