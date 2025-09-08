using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class CoinEffectManager : MonoBehaviour
{
    [SerializeField] private GameObject PileofCoinParent;
    [SerializeField] private GameObject PileofDiaParent;
    private Vector3[] InitialPos;
    private Quaternion[] InitialRotation;
    [SerializeField] private RectTransform goldAnchorPos;
    [SerializeField] private RectTransform diaAnchorPos;
    void Start()
    {
        GameManager.Instance.coinEffect = this;

        InitialPos = new Vector3[PileofCoinParent.transform.childCount];
        InitialRotation = new Quaternion[PileofCoinParent.transform.childCount];

        for (int i = 0; i < PileofCoinParent.transform.childCount; i++)
        {
            InitialPos[i] = PileofCoinParent.transform.GetChild(i).position;
            InitialRotation[i] = PileofCoinParent.transform.GetChild(i).rotation;
        }
    }

    private void ResetCoins()
    {
        for (int i = 0; i < PileofCoinParent.transform.childCount; i++)
        {
            PileofCoinParent.transform.GetChild(i).position = InitialPos[i];
            PileofCoinParent.transform.GetChild(i).rotation = InitialRotation[i];
        }
    }

    /// <summary>
    /// 코인 지급 시 이펙트를 위한 함수
    /// </summary>
    /// <param name="pre_Coin"></param>
    /// <param name="next_Coin"></param>
    /// <param name="coinType"> 0 = Gold, 1 = Dia</param>
    public void RewardPileOfCoin(int pre_Coin, int next_Coin, int coinType)
    {
        ResetCoins();

        var delay = 0f;

        switch (coinType)
        {
            case 0:
                PileofCoinParent.SetActive(true);

                for (int i = 0; i < PileofCoinParent.transform.childCount; i++)
                {
                    PileofCoinParent.transform.GetChild(i).DOScale(1f, 0.3f).SetDelay(delay).SetEase(Ease.OutBack).OnStart(() => UISoundManager.Instance.PlayClickSound());

                    PileofCoinParent.transform.GetChild(i).GetComponent<RectTransform>().DOAnchorPos(goldAnchorPos.localPosition, 1f)
                    .SetDelay(delay + 0.5f).SetEase(Ease.InBack);

                    PileofCoinParent.transform.GetChild(i).DORotate(Vector3.zero, 0.5f).SetDelay(delay + 0.5f).SetEase(Ease.Flash);

                    PileofCoinParent.transform.GetChild(i).DOScale(0f, 0.3f).SetDelay(delay + 1.5f).SetEase(Ease.OutBack);

                    delay += 0.1f;
                }
                break;

            case 1:
                PileofDiaParent.SetActive(true);

                for (int i = 0; i < PileofDiaParent.transform.childCount; i++)
                {
                    PileofDiaParent.transform.GetChild(i).DOScale(1f, 0.3f).SetDelay(delay).SetEase(Ease.OutBack).OnStart(() => UISoundManager.Instance.PlayClickSound());

                    PileofDiaParent.transform.GetChild(i).GetComponent<RectTransform>().DOAnchorPos(diaAnchorPos.localPosition, 1f)
                    .SetDelay(delay + 0.5f).SetEase(Ease.InBack);

                    PileofDiaParent.transform.GetChild(i).DORotate(Vector3.zero, 0.5f).SetDelay(delay + 0.5f).SetEase(Ease.Flash);

                    PileofDiaParent.transform.GetChild(i).DOScale(0f, 0.3f).SetDelay(delay + 1.5f).SetEase(Ease.OutBack);

                    delay += 0.1f;
                }
                break;
        }

        StartCoroutine(SetCoin(pre_Coin, next_Coin, coinType));
    }

    IEnumerator SetCoin(int pre_Value, int next_Value, int coinType)
    {
        yield return new WaitForSecondsRealtime(1f);

        float timer = 0f;
        float duration = 2f;

        UISoundManager.Instance.PlayCoinIncreaseSound();

        switch (coinType)
        {
            case 0:
                while (timer < duration)
                {
                    timer += Time.deltaTime;
                    GameManager.Instance.Gold = (int)Mathf.Lerp(pre_Value, next_Value, timer / duration);
                    yield return null;
                }

                GameManager.Instance.Gold = next_Value;
                break;

            case 1:
                while (timer < duration)
                {
                    timer += Time.deltaTime;
                    GameManager.Instance.Dia = (int)Mathf.Lerp(pre_Value, next_Value, timer / duration);
                    yield return null;
                }

                GameManager.Instance.Dia = next_Value;
                break;
        }

        UISoundManager.Instance.StopCoinIncreaseSound();

        GameManager.Instance.SavePlayerData();
    }
}
