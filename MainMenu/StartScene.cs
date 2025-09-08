using UnityEngine;
using UnityEngine.UI;

public class StartScene : MonoBehaviour
{
    private Button _playAloneBtn;
    public string loadSceneName;

    void Start()
    {
        _playAloneBtn = GetComponent<Button>();
        _playAloneBtn.onClick.AddListener(ClickSceneStartButton);
    }

    private void ClickSceneStartButton()
    {
        GameManager.Instance.StartAloneScene(loadSceneName);
    }
}
