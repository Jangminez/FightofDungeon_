using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MapPortal : MonoBehaviour
{
    [SerializeField] private Transform _monsterZone;
    [SerializeField] private Animator _anim;

    void Start()
    {
        _anim = GetComponent<Animator>();
    }
 
    private void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "Player")
        {
            StartCoroutine(TeleportToZone(col));
        }
    }

    IEnumerator TeleportToZone(Collider2D col)
    {
        // 텔레포트 애니메이션 실행
        _anim.SetTrigger("Teleport");
        yield return new WaitForSeconds(0.5f);

        // 플레이어 위치 변경 및 레이어 설정
        col.transform.position = _monsterZone.position - new Vector3(0f, 0.5f, 0f);
        col.GetComponent<SortingGroup>().sortingLayerName = "Layer 2";
    }
}
