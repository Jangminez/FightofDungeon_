using UnityEngine;
using UnityEngine.UI;

public class TriggerUI : MonoBehaviour
{
    [SerializeField]
    private GameObject _triggerUI;
    [SerializeField]
    private Text _descText;
    [TextArea(2, 5)]
    public string _description;

    void Start()
    {
        _descText.text = _description;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.layer == 17)
        {

            _triggerUI.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.layer == 17)
        {
            _triggerUI.SetActive(false);
        }
    }
}
