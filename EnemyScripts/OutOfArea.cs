using UnityEngine;

public class OutOfArea : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D col)
    {
        var enemy = col.transform.GetComponent<Enemy>();

        if (enemy != null)
        {
            enemy.state = Enemy.States.Return;
        }
    }
}
