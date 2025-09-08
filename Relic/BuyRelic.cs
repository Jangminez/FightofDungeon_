using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BuyRelic : MonoBehaviour
{
    public RandomRelicShop relicShop;
    public ScriptableRelic myRelic; // 유물 SO
    private Button myBtn; // 슬롯 버튼
    public GameObject relic_Description; // 유물 구매창
    public GameObject isBuy_Object; // 구매 완료 UI
    public bool isBuy; // 구매 확인
    public Image relic_Icon; // 유물 아이콘
    public Text relic_Name; // 유물 이름
    public Text relic_Information; // 유물 정보
    public Button buyBtn; // 유물 구매 버튼
    public int relicCost; // 유물 가격
    public int costType; // 0 골드. 1 다이아

    void Awake()
    {
        myBtn = GetComponent<Button>();
        myBtn.onClick.AddListener(ShowDescription);
        isBuy_Object = transform.GetChild(2).gameObject;
    }

    void ShowDescription()
    {
        // 구매창 초기화 및 버튼 이벤트 연결
        relic_Icon.sprite = myRelic.r_Icon;
        relic_Name.text = myRelic.r_Name;
        relic_Information.text = myRelic.r_Description;
        relic_Description.SetActive(true);

        buyBtn.onClick.RemoveAllListeners();
        buyBtn.onClick.AddListener(ClickBuyButton);
    }

    void ClickBuyButton()
    {
        if (isBuy) return;

        // 유물 구매 
        if (costType == 0)
        {
            if (GameManager.Instance.Gold >= relicCost)
            {
                GameManager.Instance.Gold -= relicCost;
                myRelic.r_Count++;

                SetBuySlot();
                relicShop.BuyItem(transform.GetSiblingIndex());

                ExitUI();
                GameManager.Instance.SavePlayerData();
            }

            else
            {
                UISoundManager.Instance.PlayCantBuySound();
            }
        }

        else
        {
            if (GameManager.Instance.Dia >= relicCost)
            {
                GameManager.Instance.Dia -= relicCost;
                myRelic.r_Count++;

                SetBuySlot();
                relicShop.BuyItem(transform.GetSiblingIndex());

                ExitUI();
                GameManager.Instance.SavePlayerData();
            }

            else
            {
                UISoundManager.Instance.PlayCantBuySound();
            }
        }
    }

    public void ResetSlot()
    {
        isBuy = false;
        isBuy_Object.SetActive(false);
        myBtn.interactable = true;
    }

    public void SetBuySlot()
    {
        myRelic.isDraw = true;
        isBuy = true;
        isBuy_Object.SetActive(true);
        myBtn.interactable = false;
    }

    private void ExitUI()
    {
        UISoundManager.Instance.PlayBuySound();

        HideUI(relic_Description.transform);
    }

    public void HideUI(Transform obj)
    {
        obj.DOScale(0, 0.3f).SetEase(Ease.InBack)
            .OnComplete(() => obj.gameObject.SetActive(false));
    }
}
