using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HideUIEffect : MonoBehaviour
{
    public enum hideType { pop, down }
    public hideType hT;
    private Button exitBtn;
    public GameObject obj;

    void Awake()
    {
        exitBtn = GetComponent<Button>();

        if (exitBtn != null)
            exitBtn.onClick.AddListener(ExitUI);
    }

    private void ExitUI()
    {
        UISoundManager.Instance.PlayExitSound();

        HideUI(obj.transform);
    }

    public void HideUI(Transform obj)
    {
        if (obj.gameObject.activeSelf)
        {
            switch (hT)
            {
                case hideType.pop:
                    obj.DOScale(0, 0.3f).SetEase(Ease.InBack)
                    .OnComplete(() => obj.gameObject.SetActive(false));
                    break;

                case hideType.down:
                    obj.DOMoveY(obj.position.y - 1200f, 0.5f).SetEase(Ease.InOutQuad) // 아래로 이동
                        .OnComplete(() => obj.gameObject.SetActive(false));
                    break;

            }
        }
    }
}
