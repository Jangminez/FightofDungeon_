using System;
using System.Collections;
using System.Threading.Tasks;
using GooglePlayGames.BasicApi.SavedGame;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            // 싱글톤 구현
            if (!_instance)
            {
                _instance = FindObjectOfType(typeof(GameManager)) as GameManager;

                if (_instance == null)
                    Debug.Log("인스턴스를 생성합니다");
            }
            return _instance;
        }
    }

    #region 플레이어 데이터
    private string nickname;
    private int level;
    private float exp;
    private float nextExp;
    private int gold;
    private int dia;
    private int winCount;
    private bool isChangeName;
    private bool didTutorial;

    public string Nickname
    {
        set
        {
            nickname = value;

            if (playerData != null)
                playerData.nickname = nickname;

            if (mainUI != null)
            {
                mainUI.SetNickName(nickname);
            }
        }

        get => nickname;
    }
    public int Level
    {
        set
        {
            level = Math.Max(0, value);

            if (playerData != null)
                playerData.level = level;

            if (mainUI != null)
                mainUI.SetLevel(value);
        }

        get => level;
    }
    public float Exp
    {
        set
        {
            exp = Math.Max(0, value);

            if (playerData != null)
                playerData.exp = exp;

            if (mainUI != null)
                mainUI.SetExpBar(playerData.exp, playerData.nextExp);

            if (exp >= nextExp)
            {
                LevelUp();
            }
        }

        get => exp;
    }
    public float NextExp
    {
        set
        {
            nextExp = Math.Max(0, value);

            if (playerData != null)
                playerData.nextExp = nextExp;
        }

        get => nextExp;
    }
    public int Gold
    {
        set
        {
            gold = Math.Max(0, value);

            if (playerData != null)
                playerData.gold = gold;

            if (mainUI != null)
                mainUI.SetGold(gold);
        }
        get => gold;
    }
    public int Dia
    {
        set
        {
            dia = Math.Max(0, value);

            if (playerData != null)
                playerData.dia = dia;

            if (mainUI != null)
                mainUI.SetDia(dia);
        }
        get => dia;
    }

    public int WinCount
    {
        set
        {
            winCount = Math.Max(0, value);

            if (playerData != null)
                playerData.winCount = winCount;

            if (mainUI != null)
                mainUI.SetWinCount(winCount);
        }

        get => winCount;
    }

    public bool IsChangeName
    {
        set
        {
            isChangeName = value;

            if (isChangeName && mainUI != null)
            {
                mainUI.canChangeFirst.SetActive(false);
                mainUI.nameEdit_Btn.interactable = false;
            }
        }
        get => isChangeName;
    }

    public bool DidTutorial { set => didTutorial = value; get => didTutorial; }
    #endregion
    [HideInInspector] public MainUIController mainUI;
    [SerializeField] GameObject quitUI;
    public string playerPrefabName;
    public Player player;
    public int rewardGold;
    public int rewardDia;
    public float rewardExp;

    public SaveSystem saveSystem;
    private PlayerData playerData;
    public CoinEffectManager coinEffect;
    public float clearTime;

    private void Awake()
    {
        // 인스턴스가 없을 때 해당 오브젝트로 설정
        if (_instance == null)
            _instance = this;

        // 인스턴스가 존재한다면 현재 오브젝트 파괴
        else if (_instance != null)
            Destroy(gameObject);

        // 씬 로드시에도 파괴되지않음 
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            quitUI.SetActive(true);
            quitUI.transform.parent.GetChild(0).gameObject.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            quitUI.SetActive(true);
            quitUI.transform.parent.GetChild(0).gameObject.SetActive(true);
        }
    }

    public void SavePlayerData()
    {
        if (playerData != null && saveSystem != null)
            saveSystem.SaveData(playerData);
    }

    private void LoadPlayerData()
    {
        playerData = saveSystem.LoadData();

        if (playerData != null)
        {
            ApplyPlayerData();
            ApplyRelicData();
        }
        else
        {
            return;
        }
    }

    public void LoadPlayerDataWithGPGS(PlayerData data)
    {
        playerData = data;

        if (playerData != null)
        {
            ApplyPlayerData();
            ApplyRelicData();
        }
        else
        {
            return;
        }
    }

    private void ApplyPlayerData()
    {
        if (playerData == null)
        {
            Debug.LogError("PlayerData가 존재하지않습니다.");
            return;
        }

        // JSON 데이터 적용
        Nickname = playerData.nickname;
        Gold = playerData.gold;
        Dia = playerData.dia;
        Level = playerData.level;
        NextExp = playerData.nextExp;
        Exp = playerData.exp;
        WinCount = playerData.winCount;
        IsChangeName = playerData.isChangeName;
        DidTutorial = playerData.didTutorial;
    }

    private void ApplyRelicData()
    {
        // 유물 정보 적용
        if (playerData == null || playerData.relicDict == null)
        {
            Debug.LogError("Relic 데이터를 불러올 수 없습니다.");
            return;
        }
        for (int i = 101; i <= 109; i++)
        {
            ScriptableRelic relic = RelicManager.Instance.GetRelic(i);

            relic.r_Level = playerData.relicDict[i].r_Level;
            relic.r_Count = playerData.relicDict[i].r_Count;
            relic.r_UpgradeCost = 2000 + (1000 * (playerData.relicDict[i].r_Level - 1));
            relic.r_UpgradeCount = 5 + (3 * (playerData.relicDict[i].r_Level - 1));
            relic.r_UpgradeValue = (playerData.relicDict[i].r_Level - 1) * 2;
        }
    }

    private void LevelUp()
    {
        if (playerData != null)
        {
            Exp -= NextExp;
            NextExp += 500f;

            Level += 1;

            mainUI.SetExpBar(Exp, NextExp);

            if (Exp >= NextExp)
            {
                LevelUp();
            }

            SavePlayerData();
            LoadPlayerData();
        }
    }

    public void BackToScene()
    {
        StartCoroutine(BackToMain());
        
        if(!DidTutorial)
        {
            DidTutorial = true;
            SavePlayerData();
        }
    }

    private IEnumerator BackToMain()
    {
        LoadingScreen.Instance.ShowLoadingScreen();

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("MainScene");

        GameLobby.Instance.LeaveLobby();
        NetworkManager.Singleton.Shutdown();
        Destroy(NetworkManager.Singleton.gameObject);

        while (!asyncOperation.isDone)
        {
            yield return null;
        }

        LoadPlayerData();

        yield return new WaitForSeconds(1f);

        LoadingScreen.Instance.HideLoadingScreen();
    }

    public void StartMainScene()
    {
        UISoundManager.Instance.PlayClickSound();

        StartCoroutine(LoadMainSceneCoroutine());
    }

    private IEnumerator LoadMainSceneCoroutine()
    {
        LoadingScreen.Instance.ShowLoadingScreen();

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("MainScene");

        while (!asyncOperation.isDone)
        {
            yield return null;
        }

        LoadPlayerData();

        yield return new WaitForSeconds(1f);

        // 튜토리얼을 한 적이 없다면 튜토리얼 씬으로 이동
        if (!didTutorial)
        {
            StartAloneScene("TutorialScene");
            yield break;
        }

        LoadingScreen.Instance.HideLoadingScreen();
    }

    public void StartAloneScene(string sceneName)
    {
        Task<string> code = ConnectRelay.Instance.CreateRelay();
        Debug.Log(code);
        StartCoroutine(LoadScene(sceneName));
    }

    private IEnumerator LoadScene(string sceneName)
    {
        LoadingScreen.Instance.ShowLoadingScreen();

        while (!NetworkManager.Singleton.IsConnectedClient)
            yield return null;

        NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);

        if (sceneName == "TutorialScene")
        {
            NetworkManager.Singleton.SceneManager.OnLoadComplete += SetTutorialPlayer;
        }
    }

    private void SetTutorialPlayer(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
    {
        if (sceneName == "TutorialScene")
        {
            player = GameObject.FindWithTag("Player").GetComponent<Player>();
            player.GetComponent<PlayerMovement>().enabled = true;

            RelicManager.Instance.ApplyRelics();
            LoadingScreen.Instance.HideLoadingScreen();
        }

        NetworkManager.Singleton.SceneManager.OnLoadComplete -= SetTutorialPlayer;
    }

    public void GameOver()
    {
        if(!DidTutorial)
            DidTutorial = true;

        NetworkManager.Singleton.Shutdown();
        Destroy(NetworkManager.Singleton.gameObject);

        if (GameLobby.Instance.joinedLobby != null)
            GameLobby.Instance.LeaveLobby();

        StartCoroutine(GameOverCoroutine());
    }

    private IEnumerator GameOverCoroutine()
    {
        LoadingScreen.Instance.ShowLoadingScreen();

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("MainScene");

        while (!asyncOperation.isDone)
        {
            yield return null;
        }

        // 데이터 불러오기
        LoadPlayerData();
        yield return new WaitForSeconds(1f);
        LoadingScreen.Instance.HideLoadingScreen();

        // 게임 종료시 골드와 경험치 지급
        coinEffect.RewardPileOfCoin(Gold, Gold + rewardGold, 0);
        coinEffect.RewardPileOfCoin(Dia, Dia + rewardDia, 1);
        Exp += rewardExp;
    }

    public void GetPowerUp()
    {
        if (player != null)
        {
            player.Attack += 800f;
            player.Defense += 500f;
            player.AttackSpeed += 2f;
            player.MaxHp += 1000f;
            player.MaxMp += 1000f;
            player.HpRegen += 100f;
            player.MpRegen += 100f;
            player.Speed += 3f;
        }
    }

    public void GameQuit()
    {
        SavePlayerData();

        Application.Quit();
    }
}
