using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawner : NetworkBehaviour
{
    [SerializeField]
    private CameraFollow _cam;
    public bool isSpawn = false;

    void Awake()
    {
        isSpawn = false;
    }
    
    public override void OnNetworkSpawn()
    {

        // 씬이 로드 되면 플레이어 오브젝트 생성
        NetworkManager.Singleton.SceneManager.OnLoadComplete += SpawnPlayerObject;
    }

    private void SpawnPlayerObject(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
    {
        if (sceneName == "StageScene" || sceneName == "PlayAloneScene")
        {
            if (IsServer)
            {
                PlayerSpawnClientRpc(clientId, sceneName);
            }
        }
    }

    [ClientRpc]
    public void PlayerSpawnClientRpc(ulong clientId, string sceneName)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId && !isSpawn)
        {
            if(IsServer)
                NetworkSpawnPlayerServerRpc(clientId, GameManager.Instance.playerPrefabName);
            
            else
                NetworkSpawnPlayerServerRpc(clientId, GameManager.Instance.playerPrefabName);

            Invoke("SpawnPlayer", 3f);
            isSpawn = true;
        }

        if(sceneName == "PlayAloneScene")
            Invoke("HideLoading", 3f);
    }

    private void HideLoading()
    {
        LoadingScreen.Instance.HideLoadingScreen();
    }

    private void SpawnPlayer()
    {
        // 각 매니저에 해당 클라이언트의 플레이어 설정
        GameManager.Instance.player = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject().GetComponent<Player>();
        UIManager.Instance.player = GameManager.Instance.player;

        //플레이어 위치 설정
        if(IsServer)
        {
            GameManager.Instance.player._spawnPoint = GameObject.FindWithTag("BlueSpawn").transform;
        }
        else
        {
            GameManager.Instance.player._spawnPoint = GameObject.FindWithTag("RedSpawn").transform;
        }

        SetUpCamera(GameManager.Instance.player.transform);
        GameManager.Instance.player.gameObject.SetActive(true);
        RelicManager.Instance.ApplyRelics();

        if(!IsServer) SceneLoadManager.Instance.HideLoadingServerRpc();
    }

    private void SetUpCamera(Transform playerTransform)
    {
        // 플레이어 카메라 설정
        _cam = GameObject.FindWithTag("MainCamera").GetComponent<CameraFollow>();
        _cam.target = playerTransform;
        _cam.transform.position = new Vector3(playerTransform.position.x, playerTransform.position.y, _cam.transform.position.z);
        _cam.offset = _cam.transform.position - _cam.target.position;
    }

    [ServerRpc(RequireOwnership = false)]
    public void NetworkSpawnPlayerServerRpc(ulong clientId, string name)
    {
        NetworkObject playerObject = Instantiate(Resources.Load<GameObject>($"PlayerCharactors/{name}")).GetComponent<NetworkObject>();

        if(clientId == NetworkManager.Singleton.LocalClientId)
            playerObject.transform.position = GameObject.FindWithTag("BlueSpawn").transform.position + new Vector3(0f, 1f, 0f) ;
        else
            playerObject.transform.position = GameObject.FindWithTag("RedSpawn").transform.position + new Vector3(0f, 1f, 0f);

        playerObject.SpawnAsPlayerObject(clientId, true);
    }

    
}
