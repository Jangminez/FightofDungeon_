using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PurchaseItem : MonoBehaviour
{
    private Button myBtn;
    public GameObject purchaseInfo;
    public Button pButton;
    public int itemValue;
    public int itemCost;

    void Awake()
    {
        myBtn = GetComponent<Button>();
        if (myBtn != null)
        {
            myBtn.onClick.AddListener(ClickBtn);
        }
    }

    private void ClickBtn()
    {
        purchaseInfo.SetActive(true);
        UISoundManager.Instance.PlayClickSound();

        if (pButton != null)
        {
            pButton.onClick.RemoveAllListeners();
            pButton.onClick.AddListener(BuyGold);
        }
    }

    private void BuyGold()
    {
        if(GameManager.Instance.Dia >= itemCost)
        {
            GameManager.Instance.Dia -= itemCost;

            int gold = GameManager.Instance.Gold;

            GameManager.Instance.coinEffect.RewardPileOfCoin(gold, gold + itemValue, 0);

            purchaseInfo.SetActive(false);
        }

        else
        {
            UISoundManager.Instance.PlayCantBuySound();
        }
    }
}


