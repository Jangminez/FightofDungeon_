using UnityEngine;

public class EnemyFindTarget : MonoBehaviour
{
    [SerializeField] private LayerMask player;
    private Collider2D[] players;
    Enemy enemy;

    void Start()
    {
        enemy = GetComponent<Enemy>();
    }

    private void FixedUpdate()
    {
        players = Physics2D.OverlapCircleAll(transform.position, enemy.stat.chaseRange, player);
        if (players.Length > 0)
        {
            float closePlayer1 = Vector2.Distance(transform.position, players[0].transform.position);

            foreach (Collider2D coll in players)
            {
                float closePlayer2 = Vector2.Distance(transform.position, coll.transform.position);

                if (closePlayer1 >= closePlayer2)
                {
                    closePlayer1 = closePlayer2;
                    enemy._target = coll.transform;
                }
            }
        }

        else
        {
            enemy._target = null;
        }
    }
}

