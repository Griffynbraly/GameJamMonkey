using UnityEngine;
using System.Collections;

public class tazeBullet : MonoBehaviour
{
    private float speed = 15;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(DeathDelay());
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }

    IEnumerator DeathDelay()
    {
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider != null)
        {
            if (collider.gameObject.name == "Player")
            {
                Debug.Log("trigger collided");
                PlayerMove playerMove = collider.GetComponent<PlayerMove>();
                if (playerMove != null)
                {
                    playerMove.HitByBullet();
                }
            }
            if (collider.gameObject.name != "AstronautGuard")
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision != null)
        {
            if (collision.gameObject.layer == 6)
            {
                Debug.Log("collider collided");
                Destroy(gameObject);
            }
        }
    }
}
