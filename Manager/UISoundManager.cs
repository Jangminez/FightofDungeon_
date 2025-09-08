using UnityEngine;

public class UISoundManager : MonoBehaviour
{
    private static UISoundManager _instance;
    public static UISoundManager Instance
    {
        get
        {
            // 싱글톤 구현
            if (!_instance)
            {
                _instance = FindObjectOfType(typeof(UISoundManager)) as UISoundManager;

                if (_instance == null)
                    Debug.Log("인스턴스를 생성합니다");
            }
            return _instance;
        }
    }

    private AudioSource _audio;
    [SerializeField] private AudioClip clickButton_Sound;
    [SerializeField] private AudioClip exitButton_Sound;
    [SerializeField] private AudioClip buy_Sound;
    [SerializeField] private AudioClip cantBuy_Sound;
    [SerializeField] private AudioClip popUp_Sound;
    [SerializeField] private AudioClip coinInc_Sound;

    void Awake()
    {
        // 인스턴스가 없을 때 해당 오브젝트로 설정
        if (_instance == null)
            _instance = this;

        // 인스턴스가 존재한다면 현재 오브젝트 파괴
        else if (_instance != null)
            Destroy(gameObject);

        // 씬 로드시에도 파괴되지않음 
        DontDestroyOnLoad(gameObject);

        _audio = GetComponent<AudioSource>();
    }

    public void PlayClickSound()
    {
        _audio.PlayOneShot(clickButton_Sound);
    }

    public void PlayExitSound()
    {
        _audio.PlayOneShot(exitButton_Sound);
    }
    
    public void PlayBuySound()
    {
        _audio.PlayOneShot(buy_Sound);
    }

    public void PlayPopUpSound()
    {
        _audio.PlayOneShot(popUp_Sound);
    }

    public void PlayCantBuySound()
    {
        _audio.PlayOneShot(cantBuy_Sound);
    }

    public void PlayOneShotSound(AudioClip clip)
    {
        _audio.PlayOneShot(clip);
    }

    public void PlayCoinIncreaseSound()
    {
        _audio.clip = coinInc_Sound;
        _audio.loop = true;
        _audio.Play();
    }

    public void StopCoinIncreaseSound()
    {
        _audio.Stop();
        _audio.loop = false;
        _audio.clip = null;
    }
}
