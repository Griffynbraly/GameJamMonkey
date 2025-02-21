using UnityEngine;
using System.Collections;

public class tazeBullet : MonoBehaviour
{
    private float speed = 15;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
            // Check if the object has only a BoxCollider2D and no other collider types
            BoxCollider2D boxCollider = collision.GetComponent<BoxCollider2D>();
            Collider2D[] colliders = collision.GetComponents<Collider2D>();

            if (boxCollider.gameObject.CompareTag("Player"))
            {

                PlayerMove playerMove = boxCollider.GetComponent<PlayerMove>();
                if (playerMove != null)
                {
                    playerMove.Killed();
                    Destroy(gameObject);
                }
            }
            else if (boxCollider != null && colliders.Length == 1)
            {
                if (boxCollider.gameObject.layer == 6)
                {
                    Destroy(gameObject);
                }
            }
        }
      
    }
}
