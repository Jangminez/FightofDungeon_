using GooglePlayGames;
using UnityEngine;
using UnityEngine.UI;

public class StageTimer : MonoBehaviour
{
    private const string leaderboardId = "CgkI2teMsLASEAIQAQ";
    public Text clearTimeUI; // 클리어 타임을 표시할 UI
    public Text countDownUI; // 제한 시간 표시할 UI
    public Slider timeSlider; // 제한 시간 슬라이더UI
    private bool isStageActive = false;
    private float remainingTime;
    public float maxTime;


    void Start()
    {
        StartStage();
    }

    void Update()
    {
        if (isStageActive)
        {
            remainingTime -= Time.deltaTime;
            if (remainingTime <= 0)
            {
                remainingTime = 0;
                EndStage(false);
            }

            countDownUI.text = FormatTime(remainingTime);
            timeSlider.value = remainingTime / maxTime;

            if (remainingTime <= 60)
            {
                timeSlider.fillRect.GetComponent<Image>().color = Color.red;
            }
        }
    }

    public void StartStage()
    {
        remainingTime = maxTime;
        isStageActive = true;
    }

    public void EndStage(bool isWin)
    {
        isStageActive = false;
        float clearTime = maxTime - remainingTime;
        clearTimeUI.text = FormatTime(clearTime);

        // 리더보드에 업로드
        if (isWin)
        {
            StageRewardManager.Instance.ShowRewardUI(isWin);
            SubmitStageClearTime(clearTime);
        }
        else
            StageRewardManager.Instance.ShowRewardUI(false);

    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        int milliseconds = Mathf.FloorToInt((time * 100) % 100);
        return string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
    }

    public void SubmitStageClearTime(float clearTime)
    {
        if(PlayGamesPlatform.Instance.IsAuthenticated())
        {
            long score = (long)(clearTime * 1000); // 초 -> 밀리초 변환
            PlayGamesPlatform.Instance.ReportScore(score, leaderboardId, (success) => 
            {
                if(success)
                    Debug.Log($"리더보드 업로드 성공:  {clearTime}초 ({score} 밀리초)");
                else
                    Debug.Log("리더보드 업로드 실패");
            });
        }

        else
        {
            Debug.Log("로그인 되지 않음 (리더보드 업로드 실패)");
        }
    }
}
