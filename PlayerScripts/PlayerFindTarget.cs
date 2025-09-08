using Unity.Netcode;
using UnityEngine;

public class PlayerFindTarget : NetworkBehaviour
{
    [SerializeField] private LayerMask layer;
    [SerializeField] private Collider2D[] enemys;
    Player player;

    public override void OnNetworkSpawn()
    {
        player = GetComponent<Player>();
    }

    private void Update()
    {
        if(player == null) return;
        
        enemys = Physics2D.OverlapCircleAll(transform.position, player.AttackRange, layer);

        if (enemys.Length > 0)
        {
            float closeEnemy1 = Vector2.Distance(transform.position, enemys[0].transform.position);

            foreach (Collider2D coll in enemys)
            {
                float closeEnemy2 = Vector2.Distance(transform.position, coll.transform.position);

                if (closeEnemy1 >= closeEnemy2)
                {
                    closeEnemy1 = closeEnemy2;
                    player._target = coll.transform;
                }
            }
        }

        else
        {
            player._target = null;
        }
    }
}

