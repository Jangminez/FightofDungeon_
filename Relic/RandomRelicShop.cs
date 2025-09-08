using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class ShopData
{
    public List<int> relicIDs = new List<int>();
    public List<int> costs = new List<int>();
    public List<int> costTypes = new List<int>();
    public string lastResetTime;
    public List<bool> isBuy = new List<bool>();
}

public class RandomRelicShop : MonoBehaviour
{
    public Sprite[] price_Icons;
    public Transform[] slots;
    public List<ScriptableRelic> shop_relics = new List<ScriptableRelic>();
    public int[] random_goldCost;
    public int[] random_diaCost;
    public Text timerText;
    public Button resetButton;
    public Button confirmBtn;
    public GameObject resetInfo;
    private const string LastResetTime = "LastShopResetTime";
    private const string ShopDataKey = "ShopData";
    private TimeSpan resetInterval = TimeSpan.FromHours(3);

    void Awake()
    {
        confirmBtn.onClick.AddListener(ClickConfirmBtn);
        resetButton.onClick.AddListener(ClickResetBtn);
    }

    void Start()
    {
        LoadShopData();
        InvokeRepeating(nameof(UpdateTimerUI), 0, 1f);
    }

    void UpdateTimerUI()
    {
        DateTime lastResetTime = GetLastResetTime();
        DateTime nextResetTime = lastResetTime.Add(resetInterval);
        TimeSpan timeRemaining = nextResetTime - DateTime.UtcNow;

        timerText.text = $"{timeRemaining.Hours:D2}:{timeRemaining.Minutes:D2}:{timeRemaining.Seconds:D2}";
    }

    DateTime GetLastResetTime()
    {
        string lastResetTimeString = PlayerPrefs.GetString(LastResetTime, "");
        return string.IsNullOrEmpty(lastResetTimeString) ? DateTime.UtcNow : DateTime.Parse(lastResetTimeString);
    }

    void LoadShopData()
    {
        string json = PlayerPrefs.GetString(ShopDataKey, "");
        if (!string.IsNullOrEmpty(json))
        {
            ShopData data = JsonUtility.FromJson<ShopData>(json);
            DateTime lastResetTime = DateTime.Parse(data.lastResetTime);
            DateTime currentTime = DateTime.UtcNow;

            if ((currentTime - lastResetTime) < resetInterval)
            {
                ApplyShopData(data);
                return;
            }
        }

        ResetRelicsToSlot();
    }

    void ApplyShopData(ShopData data)
    {
        shop_relics.Clear();
        for (int i = 0; i < slots.Length; i++)
        {
            ScriptableRelic relic = RelicManager.Instance.GetRelic(data.relicIDs[i]);
            shop_relics.Add(relic);

            slots[i].GetChild(0).GetComponent<Image>().sprite = relic.r_Icon;
            slots[i].GetChild(1).GetChild(0).GetComponent<Image>().sprite = price_Icons[data.costTypes[i]];
            slots[i].GetChild(1).GetChild(1).GetComponent<Text>().text = data.costs[i].ToString();

            BuyRelic buyRelic = slots[i].GetComponent<BuyRelic>();
            buyRelic.myRelic = relic;
            buyRelic.relicCost = data.costs[i];
            buyRelic.costType = data.costTypes[i];

            if (data.isBuy[i])
            {
                buyRelic.SetBuySlot();
            }
        }
    }

    private void ClickResetBtn()
    {
        resetInfo.SetActive(true);
    }

    private void ClickConfirmBtn()
    {
        if(GameManager.Instance.Dia >= 50)
        {
            GameManager.Instance.Dia -= 50;
            ResetRelicsToSlot();

            resetInfo.SetActive(false);
            GameManager.Instance.SavePlayerData();
        }

        else
        {
            UISoundManager.Instance.PlayCantBuySound();
        }
    }

    private void ResetRelicsToSlot()
    {
        shop_relics.Clear();

        ShopData data = new ShopData();

        foreach (Transform slot in slots)
        {
            ScriptableRelic relic = GetRandomRelic();
            int random_icon = UnityEngine.Random.Range(0, 2);
            int random_cost = RandomCost(random_icon);

            slot.GetChild(0).GetComponent<Image>().sprite = relic.r_Icon;
            slot.GetChild(1).GetChild(0).GetComponent<Image>().sprite = price_Icons[random_icon];
            slot.GetChild(1).GetChild(1).GetComponent<Text>().text = random_cost.ToString();

            shop_relics.Add(relic);
            data.relicIDs.Add(relic.r_Id);
            data.costs.Add(random_cost);
            data.costTypes.Add(random_icon);
            data.isBuy.Add(false);

            BuyRelic buyRelic = slot.GetComponent<BuyRelic>();
            buyRelic.myRelic = relic;
            buyRelic.relicCost = random_cost;
            buyRelic.costType = random_icon;
            buyRelic.ResetSlot();
        }

        data.lastResetTime = DateTime.UtcNow.ToString();
        PlayerPrefs.SetString(ShopDataKey, JsonUtility.ToJson(data));
        PlayerPrefs.SetString(LastResetTime, data.lastResetTime);
        PlayerPrefs.Save();
    }

    private ScriptableRelic GetRandomRelic()
    {
        ScriptableRelic relic = RelicManager.Instance.GetRelic(UnityEngine.Random.Range(101, 110));
        return shop_relics.Contains(relic) ? GetRandomRelic() : relic;
    }

    private int RandomCost(int n)
    {
        return n == 0 ? random_goldCost[UnityEngine.Random.Range(0, random_goldCost.Length)]
                       : random_diaCost[UnityEngine.Random.Range(0, random_diaCost.Length)];
    }

    public void BuyItem(int index)
    {
        string json = PlayerPrefs.GetString(ShopDataKey, "");
        if (!string.IsNullOrEmpty(json))
        {
            ShopData data = JsonUtility.FromJson<ShopData>(json);

            if(!data.isBuy[index])
            {
                data.isBuy[index] = true;
                PlayerPrefs.SetString(ShopDataKey, JsonUtility.ToJson(data));
            }
        }
    }
}
