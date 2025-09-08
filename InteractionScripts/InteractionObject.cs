using UnityEngine;
using UnityEngine.UI;

public class InteractionObject : MonoBehaviour
{
    public Transform _interactionCanvas;
    public Transform _interactionButton;

    public Transform[] _myUI;

    private void Awake()
    {
        _interactionCanvas.gameObject.SetActive(false);
        _interactionButton.gameObject.SetActive(false);

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 17)
        {
            _interactionCanvas.gameObject.SetActive(true);
            _interactionButton.gameObject.SetActive(true);
            _interactionButton.GetComponent<Button>().onClick.AddListener(OpenUI);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 17)
        {
            _interactionCanvas.gameObject.SetActive(false);
            _interactionButton.gameObject.SetActive(false);
            _interactionButton.GetComponent<Button>().onClick.RemoveAllListeners();          
        }
    }

    private void OpenUI()
    {
        foreach(var ui in _myUI)
        {
            ui.gameObject.SetActive(true);
        }
    }   
}
