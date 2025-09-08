using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyRelic : MonoBehaviour
{
    private UpgradeRelicController relicController;
    public ScriptableRelic myRelic;
    private Button btn;

    [Header("UI")]
    public Text r_Level;
    public Text r_Count;
    public Image r_Bar;

    void Awake()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(ClickButton);
        relicController = transform.parent.GetComponent<UpgradeRelicController>();
        myRelic.myRelic = this;

        transform.GetChild(0).GetComponent<Image>().sprite = myRelic.r_Icon;
    }

    void OnEnable()
    {
        SetUI();
    }

    void ClickButton()
    {
        relicController.ClickRelic(myRelic);
    }

    public void SetUI()
    {
        r_Level.text = $"Lv.{myRelic.r_Level}";
        r_Count.text = $"{myRelic.r_Count} / {myRelic.r_UpgradeCount}";
        r_Bar.fillAmount = (float)myRelic.r_Count / (float)myRelic.r_UpgradeCount;
    }
}
