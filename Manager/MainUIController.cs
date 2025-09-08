using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MainUIController : MonoBehaviour
{
    [SerializeField] private NickNameValidator nickValidator;  // 닉네임 유효성 검사
    public Button startButton; // 빠른시작 버튼
    public Text nameText; // 상단 프로필 이름 텍스트
    public Text goldText; // 상단 프로필 골드 텍스트
    public Text diaText; // 상단 프로필 다이아 텍스트
    public Text levelText; // 상단 프로필 레벨 텍스트
    public Slider expSlider; // 상당 프로필 경험치 바
    public Button nameEdit_Btn; // 프로필 이름 변경 버튼
    public GameObject editInfo; // 프로필 닉네임 변경 UI 오브젝트
    public Button change_Btn; // 프로필 이름 변경 최종확인 버튼
    public GameObject canChangeFirst; // 이름 변경 안했을 시 나타나는 UI 오브젝트

    [Serializable]
    public class ProfileUI // 프로필 UI에 들어가는 컴포넌트들
    {
        public Text nickName; // 프로필 닉네임 텍스트
        public Text level; // 프로필 레벨 텍스트
        public Text winCount; // 프로필 우승 횟수 텍스트
        public Text exp; // 프로필 경험치양 텍스트
        public Slider slider; // 프로필 경험치 바
    }

    [Serializable]
    public class SubMenuUI{
        public RectTransform subMenuPanel;
        public Button menuButton;
        public bool isMenuOpen = false;
    }

    public ProfileUI myPorfile = new ProfileUI();
    public SubMenuUI myMenu = new SubMenuUI();

    void Start()
    {
        // 빠른시작 버튼에 리스너 추가
        startButton.onClick.AddListener(GameLobby.Instance.QuickJoinLobby);

        // 값 변경시 UI 최신화 하기위해 게임매니저에 등록
        GameManager.Instance.mainUI = this;

        // 프로필 이름변경 관련된 버튼들 리스너 추가
        nameEdit_Btn.onClick.AddListener(OpenInfo);
        change_Btn.onClick.AddListener(ChangeNickName);

        myMenu.subMenuPanel.gameObject.SetActive(false);
        myMenu.menuButton.onClick.AddListener(ToggleMenu);

        SetNickName(GameManager.Instance.Nickname);
    }

    // 닉네임 변경시 UI 업데이트
    public void SetNickName(string name) 
    {
        nameText.text = name;
        myPorfile.nickName.text = name;
    }

    // 골드 변경시 UI 업데이트
    public void SetGold(int gold)
    {
        goldText.text = gold.ToString();
    }

    // 다이아 변경시 UI 업데이트
    public void SetDia(int dia)
    {
        diaText.text = dia.ToString();
    }

    // 레벨 변경시 UI 업데이트
    public void SetLevel(int level)
    {
        levelText.text = $"Lv. {level}";
        myPorfile.level.text = $"Lv. {level}";
    }

    // 경험치 변경시 UI 업데이트
    public void SetExpBar(float exp, float nextExp)
    {
        expSlider.value = exp / nextExp;
        myPorfile.exp.text = exp.ToString("F0") + "/" + nextExp.ToString("F0");
        myPorfile.slider.value = exp / nextExp;
    }

    //우승 횟수 변경시 UI 업데이트
    public void SetWinCount(int count)
    {
        myPorfile.winCount.text = $"승리 횟수: {count}";
    }

    // 프로필 닉네임 변경 정보창 활성화
    private void OpenInfo()
    {
        editInfo.SetActive(true);
    }

    // 닉네임 변경
    private void ChangeNickName()
    {
        // 닉네임이 유효하고 InputField의 값도 변경되지않고 유효하다면 닉네임 변경
        if(nickValidator.canChange && nickValidator.IsValidNickName(nickValidator.inputField.text))
        {
            GameManager.Instance.Nickname = nickValidator.inputField.text;
            GameManager.Instance.IsChangeName = true;

            nickValidator.inputField.text = "";
            change_Btn.interactable = false;
            editInfo.SetActive(false);
    
            GameManager.Instance.SavePlayerData();
        }

        else
        {
            UISoundManager.Instance.PlayCantBuySound();
        }
    }

    public void ToggleMenu()
    {
        UISoundManager.Instance.PlayClickSound();

        if (myMenu.isMenuOpen)
        {
            // 메뉴 닫기
            myMenu.subMenuPanel.DOAnchorPosY(-35f, 0.3f).SetEase(Ease.InBack)
                .OnComplete(() => myMenu.subMenuPanel.gameObject.SetActive(false));

            
        }
        else
        {
            // 메뉴 열기
            myMenu.subMenuPanel.gameObject.SetActive(true);
            myMenu.subMenuPanel.DOAnchorPosY(-405, 0.3f).SetEase(Ease.OutBack);
        }

        myMenu.isMenuOpen = !myMenu.isMenuOpen;
    }
}
