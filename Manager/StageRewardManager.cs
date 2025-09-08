using UnityEngine;
using UnityEngine.UI;

public class StageRewardManager : MonoBehaviour
{
    private static StageRewardManager _instance;
    public static StageRewardManager Instance
    {
        get
        {
            // 싱글톤 구현
            if (!_instance)
            {
                _instance = FindObjectOfType(typeof(StageRewardManager)) as StageRewardManager;

                if (_instance == null)
                    Debug.Log("인스턴스를 생성합니다");
            }
            return _instance;
        }
    }
    public GameObject victoryUI;
    public Text victoryGold;
    public Text victoryDia;
    public Text victoryExp;
    public Button victoryBtn;
    public GameObject loseUI;
    public Text loseGold;
    public Text loseDia;
    public Text loseExp;
    public Button loseBtn;
    private int basicGold;
    private int basicDia;
    private int basicExp;
    public int playerLevel;

    public int rewardGold {get => basicGold + (playerLevel * 80);}
    public int rewardDia {get => basicDia + (playerLevel * 5);}
    public float rewardExp {get => basicExp + (playerLevel * 30f);}

    void Awake()
    {
        // 인스턴스가 없을 때 해당 오브젝트로 설정
        if (_instance == null)
            _instance = this;

        // 인스턴스가 존재한다면 현재 오브젝트 파괴
        else if (_instance != null)
            Destroy(gameObject);
    }

    void Start()
    {
        basicGold = 100;
        basicExp = 50;

        victoryBtn.onClick.AddListener(GameManager.Instance.GameOver);
        loseBtn.onClick.AddListener(GameManager.Instance.GameOver);
    }

    public void ShowRewardUI(bool isWin)
    {
        if(isWin)
        {
            victoryGold.text = (rewardGold * 1.5f).ToString("F0");
            victoryDia.text = (rewardDia * 1.5f).ToString("F0");
            victoryExp.text = (rewardExp * 1.5f).ToString("F0");
            victoryUI.SetActive(true);
        }

        else
        {
            loseGold.text = (rewardGold * 0.5f).ToString("F0");
            loseDia.text = (rewardDia * 0.5f).ToString("F0");
            loseExp.text = (rewardExp * 0.5f).ToString("F0");
            loseUI.SetActive(true);
        }

        GiveReward(isWin);
    }


    public void GiveReward(bool isWin)
    {
        if(isWin)
        {
            GameManager.Instance.rewardGold = (int)(rewardGold * 1.5f);
            GameManager.Instance.rewardDia = (int)(rewardDia * 1.5f);
            GameManager.Instance.rewardExp = rewardExp * 1.5f;
            GameManager.Instance.WinCount++;
        }

        else
        {
            GameManager.Instance.rewardGold = (int)(rewardGold * 0.5f);
            GameManager.Instance.rewardDia = (int)(rewardDia * 0.5f);
            GameManager.Instance.rewardExp = rewardExp * 0.5f;
        }
    }
}
