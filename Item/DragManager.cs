using UnityEngine;

public class DragManager : MonoBehaviour
{
    public Transform[] DropZones;

    void Update()
    {
        if(UIManager.Instance.isDragItem)
        {
            foreach(var dz in DropZones)
            {
                dz.gameObject.SetActive(true);
            }
        }        

        else
        {
            foreach(var dz in DropZones)
            {
                dz.gameObject.SetActive(false);
            }
        }
    }
}
