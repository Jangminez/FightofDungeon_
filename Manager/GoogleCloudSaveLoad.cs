using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GoogleCloudSaveLoad : MonoBehaviour
{
    [SerializeField] Button saveBtn;
    [SerializeField] Button loadBtn;
    [SerializeField] CanvasGroup successInfo;
    [SerializeField] CanvasGroup failedInfo;

    void Awake()
    {
        saveBtn.onClick.AddListener(SaveDataGPGS);
        loadBtn.onClick.AddListener(LoadDataGPGS);
    }

    private void SaveDataGPGS()
    {
        PlayerData data = SaveSystem.Instance.LoadData();

        SaveSystem.Instance.SaveDataWithGPGS(data, (isSuccess) =>
        {
            if (isSuccess)
            {
                successInfo.transform.GetChild(0).GetComponent<Text>().text = "데이터 저장 성공";

                successInfo.DOFade(1f, 1f)
                .OnComplete(() =>
                DOVirtual.DelayedCall(1f, () => successInfo.DOFade(0f, 1f).SetEase(Ease.InOutSine)));

                Debug.Log("데이터 저장 성공!!!!!!");
            }
            else
            {
                failedInfo.transform.GetChild(0).GetComponent<Text>().text = "데이터 저장 실패";

                failedInfo.DOFade(1f, 1f)
                .OnComplete(() =>
                DOVirtual.DelayedCall(1f, () => failedInfo.DOFade(0f, 1f).SetEase(Ease.InOutSine)));

                Debug.Log("데이터 저장 실패........");
            }
        });
    }

    private void LoadDataGPGS()
    {
        SaveSystem.Instance.LoadDataWithGPGS((data) =>
        {
            if (data != null)
            {
                successInfo.transform.GetChild(0).GetComponent<Text>().text = "데이터 불러오기 성공";

                successInfo.DOFade(1f, 1f)
                .OnComplete(() =>
                DOVirtual.DelayedCall(1f, () => successInfo.DOFade(0f, 1f).SetEase(Ease.InOutSine)));

                GameManager.Instance.LoadPlayerDataWithGPGS(data);

                Debug.Log("데이터 불러오기 성공!!!!!!");
            }

            else
            {
                failedInfo.transform.GetChild(0).GetComponent<Text>().text = "데이터 불러오기 실패";

                failedInfo.DOFade(1f, 1f)
                .OnComplete(() =>
                DOVirtual.DelayedCall(1f, () => failedInfo.DOFade(0f, 1f).SetEase(Ease.InOutSine)));

                Debug.Log("데이터 불러오기 실패.......");
            }
        });
    }
}
