using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using GooglePlayGames.BasicApi.SavedGame;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using System.Text;
using WebSocketSharp;

[Serializable]
public class RelicData
{
    public int r_Level;
    public int r_Count;
}

[Serializable]
public class PlayerData
{
    public string nickname;
    public int level;
    public float exp;
    public float nextExp;
    public int gold;
    public int dia;
    public int winCount;
    public bool isChangeName;
    public bool didTutorial;
    public Dictionary<int, RelicData> relicDict = new Dictionary<int, RelicData>();
}

public class SaveSystem : MonoBehaviour
{
    private static SaveSystem _instance;
    public static SaveSystem Instance
    {
        get
        {
            // 싱글톤 구현
            if (!_instance)
            {
                _instance = FindObjectOfType(typeof(SaveSystem)) as SaveSystem;

                if (_instance == null)
                    Debug.Log("인스턴스를 생성합니다");
            }
            return _instance;
        }
    }
    private string filePath;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;

        else if (_instance != null)
            Destroy(gameObject);

        filePath = Path.Combine(Application.persistentDataPath, "Player_Data.json");
        Debug.Log("데이터 저장경로 " + filePath);

        DontDestroyOnLoad(gameObject);
    }

    public void SaveDataWithGPGS(PlayerData data, Action<bool> onComplete)
    {
        SavePlayerData(data);
        SaveRelicData(data);

        string json = JsonConvert.SerializeObject(data, Formatting.Indented);

        GPGSManager.Instance.SaveGameData(json, (success) =>
        {
            onComplete?.Invoke(success);
        });
    }
    public void SaveData(PlayerData data)
    {
        SavePlayerData(data);
        SaveRelicData(data);

        string json = JsonConvert.SerializeObject(data, Formatting.Indented);

        File.WriteAllText(filePath, json);
        Debug.Log("JSON 데이터 저장 완료 경로: " + filePath);
    }

    public void SaveDataFirst(PlayerData data)
    {
        string json = JsonConvert.SerializeObject(data, Formatting.Indented);

        File.WriteAllText(filePath, json);
    }

    private void SavePlayerData(PlayerData data)
    {
        data.nickname = GameManager.Instance.Nickname;
        data.level = GameManager.Instance.Level;
        data.exp = GameManager.Instance.Exp;
        data.nextExp = GameManager.Instance.NextExp;
        data.gold = GameManager.Instance.Gold;
        data.dia = GameManager.Instance.Dia;
        data.winCount = GameManager.Instance.WinCount;
        data.isChangeName = GameManager.Instance.IsChangeName;
        data.didTutorial = GameManager.Instance.DidTutorial;
    }

    public void SaveRelicData(PlayerData data)
    {
        for (int i = 101; i <= 109; i++)
        {
            ScriptableRelic relic = RelicManager.Instance.GetRelic(i);

            if (!data.relicDict.ContainsKey(i))
            {
                data.relicDict[i] = new RelicData
                {
                    r_Level = 1,
                    r_Count = 0
                };
            }
            data.relicDict[i].r_Level = relic.r_Level;
            data.relicDict[i].r_Count = relic.r_Count;
        }
    }

    public void LoadDataWithGPGS(Action<PlayerData> onComplete)
    {
        GPGSManager.Instance.LoadGameData((json) =>
        {
            if (!json.IsNullOrEmpty())
            {
                PlayerData data = JsonConvert.DeserializeObject<PlayerData>(json);
                onComplete?.Invoke(data);
            }

            else
            {
                Debug.LogWarning("구글에 저장된 데이터 없음 불러오기 실패");
                onComplete?.Invoke(null);
            }
        });
    }

    public PlayerData LoadData()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            PlayerData data = JsonConvert.DeserializeObject<PlayerData>(json);
            Debug.Log("JSON 데이터 불러오기 경로: " + filePath);

            return data;
        }

        else
        {
            Debug.LogWarning("저장된 JSON 데이터 없음, 새로 생성");
            return CreateNewPlayerData();
        }
    }

    private PlayerData CreateNewPlayerData()
    {
        PlayerData newData = new PlayerData();

        newData.nickname = "플레이어";
        newData.level = 1;
        newData.nextExp = 1000f;
        newData.exp = 0;
        newData.gold = 0;
        newData.dia = 0;
        newData.winCount = 0;
        newData.isChangeName = false;
        newData.didTutorial = false;
        newData.relicDict = new Dictionary<int, RelicData>();

        for (int i = 101; i <= 109; i++)
        {
            newData.relicDict[i] = new RelicData
            {
                r_Level = 1,
                r_Count = 0
            };
        }

        SaveDataFirst(newData);

        return newData;
    }
}
