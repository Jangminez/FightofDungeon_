using GooglePlayGames;
using UnityEngine;
using UnityEngine.UI;

public class LeaderBoardManager : MonoBehaviour
{
    private const string leaderboardId = "CgkI2teMsLASEAIQAQ";
    Button showButton;

    void Awake()
    {
        showButton = GetComponent<Button>();
        showButton.onClick.AddListener(ShowLeaderboard);
    }

    public void ShowLeaderboard()
    {
        if(PlayGamesPlatform.Instance.IsAuthenticated())
        {
            PlayGamesPlatform.Instance.ShowLeaderboardUI(leaderboardId);
        }

        else
        {
            Debug.Log("로그인 되지 않음 (리더보드 열 수 없음)");
        }
    }
}
