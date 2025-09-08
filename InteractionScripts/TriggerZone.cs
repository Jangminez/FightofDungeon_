using UnityEngine;
using UnityEngine.Events;

public class TriggerZone : MonoBehaviour
{
    public UnityEvent onEnter;

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.transform.tag == "Player")
            onEnter.Invoke();
    }

}
