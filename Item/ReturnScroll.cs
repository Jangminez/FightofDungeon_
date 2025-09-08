using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ReturnScroll : MonoBehaviour, IConsumable
{
    public void UseItem()
    {
        UISoundManager.Instance.PlayClickSound();
        
        GameManager.Instance.player.transform.position = GameManager.Instance.player._spawnPoint.position + new Vector3(0f, 1f, 0f);
        GameManager.Instance.player.GetComponent<SortingGroup>().sortingLayerName = "Layer 1";
    }
}
