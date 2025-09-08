using System;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class GameLobby : MonoBehaviour
{
    public static GameLobby Instance { get; private set; }
    Lobby hostLobby;
    public Lobby joinedLobby;
    float heartbeatTimer;
    float HandleLobbyTimer;
    string playerName;
    public const string KEY_START_GAME = "Relay Code";
    public event EventHandler<EventArgs> OnGameStarted;
    private List<Unity.Services.Lobbies.Models.Player> previousPlayerList;
    private bool isGameStart = false;


    [Header("MyUI")]
    public Button joinButton;
    public Text roomCode;
    public InputField inputField;
    public GameObject waitingPlayer;

    private void Awake()
    {
        Instance = this;

    }

    public void SetPlayerName(string name)
    {
        playerName = name;
    }

    private async void Start()
    {
        // 코드 비동기 실행
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
        };
        // 익명 로그인
        if (!AuthenticationService.Instance.IsSignedIn)
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

        joinButton.onClick.AddListener(JoinLobbyByCode);

        playerName = GameManager.Instance.Nickname;
    }

    void Update()
    {
        if (joinedLobby != null)
        {
            HandleLobbyHeartBeat();
            HandleLobbyPollForUpdates();
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        StartGame();
    }



    // 이 신호를 통해 로비가 활성화 되어있는지 확인
    private async void HandleLobbyHeartBeat()
    {
        if (hostLobby != null)
        {
            heartbeatTimer -= Time.deltaTime;
            if (heartbeatTimer < 0f)
            {
                float heartbeatTimerMax = 15f;
                heartbeatTimer = heartbeatTimerMax;

                await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
            }
        }
    }

    // 로비의 정보가 업데이트 된다면 동기화를 위함
    private async void HandleLobbyPollForUpdates()
    {
        if (joinedLobby != null)
        {
            HandleLobbyTimer -= Time.deltaTime;
            if (HandleLobbyTimer < 0f)
            {
                float HandleLobbyTimerMax = 1.1f;
                HandleLobbyTimer = HandleLobbyTimerMax;

                joinedLobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);

                CheckPlayerCount();

                // 릴레이 코드가 추가 된다면 게임 시작
                if (joinedLobby.Data[KEY_START_GAME].Value != "0" && !isGameStart)
                {
                    ConnectRelay.Instance.JoinRelay(joinedLobby.Data[KEY_START_GAME].Value);
                    joinedLobby = null;
                    OnGameStarted?.Invoke(this, EventArgs.Empty);
                }
            }
        }
    }

    private void CheckPlayerCount()
    {
        // 연결된 플레이어의 수를 확인 후 조건이 충족되면 게임시작
        if (previousPlayerList == null)
        {
            previousPlayerList = new List<Unity.Services.Lobbies.Models.Player>(joinedLobby.Players);
            return;
        }

        if (joinedLobby.Players.Count > previousPlayerList.Count)
        {
            if (IsLobbyHost())
                StartGame();
        }
    }

    public async void CreateLobby()
    {
        // 로비 생성 및 설정
        try
        {
            string lobbyName = "GameLobby" + UnityEngine.Random.Range(0, 100).ToString();
            int maxPlayers = 2;

            CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions
            {
                IsPrivate = true,  // true로 변경 시 비공개 로비 생성 (입장코드를 통해서만 입장가능)
                Player = GetPlayer(),
                Data = new Dictionary<string, DataObject> {
                    { KEY_START_GAME, new DataObject(DataObject.VisibilityOptions.Member, "0")}
                }
            };

            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, createLobbyOptions);

            hostLobby = lobby;
            joinedLobby = hostLobby;

            roomCode.text = lobby.LobbyCode;

            Debug.Log("Create Lobby! " + lobby.LobbyCode);
        }

        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void CreateQuickLobby()
    {
        // 로비 생성 및 설정
        try
        {
            string lobbyName = "GameLobby" + UnityEngine.Random.Range(0, 100).ToString();
            int maxPlayers = 2;

            CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions
            {
                IsPrivate = false,  // true로 변경 시 비공개 로비 생성 (입장코드를 통해서만 입장가능)
                Player = GetPlayer(),
                Data = new Dictionary<string, DataObject> {
                    { KEY_START_GAME, new DataObject(DataObject.VisibilityOptions.Member, "0")}
                }
            };

            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, createLobbyOptions);

            hostLobby = lobby;
            joinedLobby = hostLobby;

            waitingPlayer.SetActive(true);

            Debug.Log("Create Lobby! " + lobby.LobbyCode);
        }

        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    // 현재 존재하는 입장가능한 로비로 빠르게 입장
    public async void QuickJoinLobby()
    {
        try
        {
            JoinLobbyByCodeOptions joinLobbyByCodeOptions = new JoinLobbyByCodeOptions
            {
                Player = GetPlayer()
            };

            Lobby lobby = await LobbyService.Instance.QuickJoinLobbyAsync();
            joinedLobby = lobby;
        }

        catch (LobbyServiceException e)
        {
            Debug.Log(e.ErrorCode);

            if(e.ErrorCode == 16006)
            {
                // 참여 가능한 로비가 없다면 공개로비 생성
                CreateQuickLobby();
            }
        }
    }

    public async void JoinLobbyByCode()
    {
        try
        {
            JoinLobbyByCodeOptions joinLobbyByCodeOptions = new JoinLobbyByCodeOptions
            {
                Player = GetPlayer()
            };

            Lobby lobby = await Lobbies.Instance.JoinLobbyByCodeAsync(inputField.text);
            joinedLobby = lobby;

            Debug.Log("Joined Lobby with code " + inputField.text);
        }

        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    // 로비에 존재하는 플레이어의 정보 가져오기
    private Unity.Services.Lobbies.Models.Player GetPlayer()
    {
        return new Unity.Services.Lobbies.Models.Player
        {
            Data = new Dictionary<string, PlayerDataObject> {
                        {"PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName)}
                    }
        };
    }

    // 로비에 존재하는 플레이어 출력
    [ContextMenu("PrintPlayer")]
    public void PrintPlayers()
    {
        Debug.Log("Players in Lobby " + joinedLobby.LobbyCode);
        foreach (var player in joinedLobby.Players)
        {
            Debug.Log(player.Id + " " + player.Data["PlayerName"].Value);
        }
    }

    // 로비 퇴장
    public async void LeaveLobby()
    {
        try
        {
            if (joinedLobby != null)
            {
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
                Debug.Log($"Leave Lobby LobbyId : {joinedLobby.LobbyCode}");
                joinedLobby = null;
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private async void MigrateLobbyHost()
    {
        try
        {
            hostLobby = await Lobbies.Instance.UpdateLobbyAsync(hostLobby.Id, new UpdateLobbyOptions
            {
                HostId = joinedLobby.Players[1].Id
            });
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private bool IsLobbyHost()
    {
        if (joinedLobby.HostId == AuthenticationService.Instance.PlayerId)
            return true;
        else
            return false;
    }

    public async void StartGame()
    {
        if (IsLobbyHost() && !isGameStart)
        {
            try
            {
                isGameStart = true;
                Debug.Log("StartGame");

                string relayCode = await ConnectRelay.Instance.CreateRelay();

                Lobby lobby = await Lobbies.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions
                {
                    Data = new Dictionary<string, DataObject> {
                    { KEY_START_GAME, new DataObject(DataObject.VisibilityOptions.Member, relayCode)}
                }
                });

                joinedLobby = lobby;

                SceneLoadManager.Instance.LoadSceneAsync("StageScene");
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

}
