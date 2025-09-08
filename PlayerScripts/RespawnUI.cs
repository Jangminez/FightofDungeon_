using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RespawnUI : MonoBehaviour
{
    public Text _countDown;
    public Text _respawnText;

    private int _count;
    private void OnEnable()
    {
        _count = 10;
        StartCoroutine("CountDown");
    }


    IEnumerator CountDown()
    {
        while (_count > 0)
        {
            _countDown.text = _count.ToString();
            _count -= 1;

            yield return new WaitForSeconds(1);
        }
        
        this.gameObject.SetActive(false);
    }
}
