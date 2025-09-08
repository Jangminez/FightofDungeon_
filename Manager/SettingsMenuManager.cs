using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenuManager : MonoBehaviour
{
    public Dropdown _frameDropdown;
    public Slider _musicVol, _sfxVol;
    public AudioMixer _mainAudioMixer;
    private GameObject settingObject;

    void Awake()
    {
        settingObject = transform.GetChild(0).GetComponent<GameObject>();
    }
    void Start()
    {
        InitializeSetting();
    }

    public void InitializeSetting()
    {
        _mainAudioMixer.SetFloat("MusicVol", PlayerPrefs.GetFloat("MusicVol", 0f));
        _mainAudioMixer.SetFloat("SfxVol", PlayerPrefs.GetFloat("SfxVol", 0f));
        int savedFrameRate = PlayerPrefs.GetInt("FrameRate", 1);
        SetFrameRate(savedFrameRate);

        _musicVol.value = PlayerPrefs.GetFloat("MusicVol", 0f);
        _sfxVol.value = PlayerPrefs.GetFloat("SfxVol", 0f);
        _frameDropdown.value = savedFrameRate;
        _frameDropdown.onValueChanged.AddListener(SetFrameRate);
    }

    public void SetFrameRate(int index)
    {
        if (settingObject != null && !settingObject.activeSelf) return;

        UISoundManager.Instance.PlayClickSound();

        switch (index)
        {
            case 0: Application.targetFrameRate = 30; break;
            case 1: Application.targetFrameRate = 60; break;
        }

        QualitySettings.vSyncCount = 0;

        PlayerPrefs.SetInt("FrameRate", index);
        PlayerPrefs.Save();
    }

    public void ChangeMusicVolume()
    {
        _mainAudioMixer.SetFloat("MusicVol", _musicVol.value);

        PlayerPrefs.SetFloat("MusicVol", _musicVol.value);
        PlayerPrefs.Save();
    }

    public void ChangeSfxVolume()
    {
        _mainAudioMixer.SetFloat("SfxVol", _sfxVol.value);

        PlayerPrefs.SetFloat("SfxVol", _sfxVol.value);
        PlayerPrefs.Save();
    }

    public void GotoMain()
    {
        UISoundManager.Instance.PlayClickSound();
        GameManager.Instance.BackToScene();
    }
}
