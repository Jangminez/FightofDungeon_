using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RelicDraw : MonoBehaviour
{
    private Button myBtn;
    public GameObject infoObject;
    public GameObject drawEffect;
    public GameObject drawInfo;
    public Text drawText;
    public Image relicIcon;
    public Text relicName;
    public Text relicCount;
    public Text relicLevel;
    public Image relicBar;

    private bool isDraw = false;

    void Awake()
    {
        myBtn = GetComponent<Button>();

        if (myBtn != null)
        {
            myBtn.onClick.AddListener(DrawRelic);
        }
    }

    private int GetRandomRelic()
    {
        return Random.Range(101, 110);
    }

    public void DrawRelic()
    {
        if (!isDraw && GameManager.Instance.Dia >= 100)
        {
            UISoundManager.Instance.PlayClickSound();
            
            drawEffect.transform.parent.gameObject.SetActive(true);
            StartCoroutine(StartDrawRelic());
            GameManager.Instance.Dia -= 100;
            isDraw = true;
        }

        else
        {
            UISoundManager.Instance.PlayCantBuySound();
        }
    }

    IEnumerator StartDrawRelic()
    {
        ScriptableRelic drawRelic = RelicManager.Instance.GetRelic(GetRandomRelic());

        if (drawRelic != null)
        {
            Debug.Log(drawRelic.r_Name);

            drawRelic.r_Count += 1;
            drawRelic.isDraw = true;

            relicIcon.sprite = drawRelic.r_Icon;
            relicName.text = drawRelic.r_Name;
            relicCount.text = $"{drawRelic.r_Count} / {drawRelic.r_UpgradeCount}";
            relicLevel.text = $"Lv. {drawRelic.r_Level}";
            relicBar.fillAmount = (float)drawRelic.r_Count / (float)drawRelic.r_UpgradeCount;
        }
        
        drawEffect.SetActive(true);

        drawEffect.GetComponent<Animator>().SetTrigger("Draw");
        drawText.text = "유물 뽑는 중";
        yield return new WaitForSeconds(0.3f);
        drawText.text = "유물 뽑는 중.";
        yield return new WaitForSeconds(0.3f);
        drawText.text = "유물 뽑는 중..";
        yield return new WaitForSeconds(0.2f);
        drawText.text = "유물 뽑는 중...";
        yield return new WaitForSeconds(0.2f);

        drawEffect.SetActive(false);



        drawInfo.SetActive(true);

        yield return null;

        isDraw = false;
        infoObject.SetActive(false);

        // 플레이어 데이터 저장
        GameManager.Instance.SavePlayerData();
    }
}
