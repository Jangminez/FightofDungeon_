using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NetworkChecker : MonoBehaviour
{
    public GameObject networkInfo;
    public Button quitBtn;
    void Start()
    {
        quitBtn.onClick.AddListener(QuitGame);
        StartCoroutine(CheckInternetConnection());
    }

    IEnumerator CheckInternetConnection()
    {
        using (UnityWebRequest request = UnityWebRequest.Get("https://www.google.com"))
        {
            yield return request.SendWebRequest();

            if(request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("인터넷 연결안 됨");
                networkInfo.SetActive(true);
                networkInfo.transform.parent.GetChild(0).gameObject.SetActive(true);
            }
            else
            {
                Debug.Log("인터넷 연결됨");
            }
        }
    }

    void QuitGame()
    {
        Application.Quit();
    }
}
