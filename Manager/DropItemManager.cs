using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;

public class DropItemManager : NetworkBehaviour
{
    private static DropItemManager _instance;
    public static DropItemManager Instance
    {
        get
        {
            // 싱글톤 구현
            if (!_instance)
            {
                _instance = FindObjectOfType(typeof(DropItemManager)) as DropItemManager;

                if (_instance == null)
                    Debug.Log("인스턴스를 생성합니다");
            }
            return _instance;
        }
    }

    [SerializeField] private GameObject chest;

    private void Awake()
    {
        // 인스턴스가 없을 때 해당 오브젝트로 설정
        if (_instance == null)
            _instance = this;

        // 인스턴스가 존재한다면 현재 오브젝트 파괴
        else if (_instance != null)
            Destroy(gameObject);
    }

    [ServerRpc(RequireOwnership = false)]
    public void DropItemServerRpc(Vector3 position, int itemId, int layerId, ServerRpcParams rpcParams = default)
    {
        // 서버에서 아이템 드롭박스 생성
        GameObject chestItem = Instantiate(chest);
        chestItem.transform.position = position;
        chestItem.GetComponent<SortingGroup>().sortingLayerID = layerId;
        chestItem.GetComponent<NetworkObject>().Spawn(true);
        DropItemClientRpc(itemId, chestItem.GetComponent<NetworkObject>().NetworkObjectId, rpcParams.Receive.SenderClientId);
    }

    [ClientRpc]
    public void DropItemClientRpc(int itemId, ulong objectId, ulong clientId)
    {
        // 각 클라이언트에서 값 할당
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(objectId, out var chestItem))
        {
            chestItem.GetComponent<DropItemChest>()._item = ItemManager.Instance.GetItem(itemId);
        }
        else
        {
            Debug.Log("드랍박스를 가져오지 못 했습니다.");
        }
    }
}
