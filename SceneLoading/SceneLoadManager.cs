using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadManager : NetworkBehaviour
{
    public static SceneLoadManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadSceneAsync(string sceneName)
    {
        if (IsServer)
        {
            StartCoroutine(LoadSceneCoroutine(sceneName));
        }
    }

    private IEnumerator LoadSceneCoroutine(string sceneName)
    {
        NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        yield return null;
    }

    [ClientRpc]
    public void ShowLoadingClientRpc()
    {
        LoadingScreen.Instance.ShowLoadingScreen();
    }

    [ServerRpc(RequireOwnership = false)]
    public void HideLoadingServerRpc()
    {
        HideLoadingClientRpc();
    }


    [ClientRpc]
    public void HideLoadingClientRpc()
    {
        LoadingScreen.Instance.HideLoadingScreen();
    }
}
