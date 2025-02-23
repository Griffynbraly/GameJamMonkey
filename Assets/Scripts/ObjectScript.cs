using UnityEngine;
using System.Collections.Generic;

public class ObjectScript : MoldBaseScript
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (GetComponent<Collider2D>() != null)
        {
            if (collision.gameObject.CompareTag("Player") && boxCollider.size == new Vector2(1, 1))
            {
                BananaPlacer placer = collision.gameObject.GetComponent<BananaPlacer>();
                if (placer != null)
                {
                    placer.hasBanana = true;
                    Destroy(gameObject);
                }
            }
        }
    }
}
