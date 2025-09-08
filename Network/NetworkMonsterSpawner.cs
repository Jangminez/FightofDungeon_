using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkMonsterSpawner : NetworkBehaviour
{
    static NetworkMonsterSpawner _instance;
    public static NetworkMonsterSpawner Instance {get {return _instance;} }
    public List<SpawnArea> spawnAreas;
    private Dictionary<GameObject, List<NetworkObject>> activeMonsters = new Dictionary<GameObject, List<NetworkObject>>();

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        if(_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        foreach (var area in spawnAreas)
        {
            activeMonsters[area.monsterPrefab] = new List<NetworkObject>();
            StartCoroutine(SpawnMonstersInArea(area));
        }
    }

    private IEnumerator<WaitForSeconds> SpawnMonstersInArea(SpawnArea area)
    {
        // 2.5초 마다 해당 구역에 몬스터 수가 최대 몬스터 수보다 작다면 몬스터 소환
        if (!IsServer) yield break;

        while (true)
        {
            if (NetworkObjectPool.Instance.isInitialized && activeMonsters[area.monsterPrefab].Count < area.maxCount)
            {
                // 구역 안에 랜덤 위치에 소환
                Vector3 spawnPosition = GetRandomPositionInRange(area.spawnCenter, area.spawnRadius);
                // ObjectPool에서 몬스터 오브젝트 가져옴
                NetworkObject monster = NetworkObjectPool.Instance.GetNetworkObject(area.monsterPrefab, spawnPosition, Quaternion.identity);

                if (!monster.IsSpawned)
                {
                    // 몬스터가 NetworkObject Spawn이 되어있지않을 때 소환
                    monster.GetComponent<Enemy>().prefab = area.monsterPrefab;
                    monster.Spawn(true);
                    monster.TrySetParent(area.spawnCenter.parent, true);

                    yield return null;
                }
                
                // 몬스터 초기화 및 활성화 몬스터 List에 저장
                monster.GetComponent<Enemy>().InitMonster();
                activeMonsters[area.monsterPrefab].Add(monster);
            }
            yield return new WaitForSeconds(2.5f);
        }
    }

    private Vector3 GetRandomPositionInRange(Transform center, float radius)
    {
        Vector2 randomPoint = Random.insideUnitCircle * radius;
        return new Vector3(center.position.x + randomPoint.x, center.position.y + randomPoint.y, 0f);
    }

    public void DespawnMonster(NetworkObject monster, GameObject prefab)
    {
        if(!IsServer) return;

        if (activeMonsters.ContainsKey(prefab) && activeMonsters[prefab].Contains(monster))
        {
            // 활성 몬스터에서 제거 및 NetworkObjectPool로 다시 리턴
            activeMonsters[prefab].Remove(monster);
            NetworkObjectPool.Instance.ReturnNetworkObject(monster, prefab);
        }
    }

    [System.Serializable]
    public class SpawnArea
    {
        public GameObject monsterPrefab;
        public int maxCount;
        public Transform spawnCenter;
        public float spawnRadius;
    }
}

