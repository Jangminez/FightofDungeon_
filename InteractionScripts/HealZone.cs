using UnityEngine;
using System.Collections;
using Unity.Netcode;

public class HealZone : MonoBehaviour
{
    bool stay = false;
    void OnTriggerEnter2D(Collider2D col)
    {
        Player player = col.transform.GetComponent<Player>();

        if (player != null && !stay)
        {
            Debug.Log("힐링 중");
            stay = true;
            StartCoroutine(Heal(player));
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        stay = false;
    }

    IEnumerator Heal(Player player)
    {
        ulong clientId = player.GetComponent<NetworkObject>().OwnerClientId;
        
        while (stay && clientId == NetworkManager.Singleton.LocalClientId)
        {

            yield return new WaitForSeconds(1f);
            Debug.Log("체력 회복, 마나 회복");
            player.Hp += player.FinalHp * 0.2f;
            player.Mp += player.FinalMp * 0.2f;

        }
    }



}
