using UnityEngine;

public class BananaScript : MonoBehaviour
{
    private GameObject player; 
    public void Start()
    {
        Vector3 localScale = transform.localScale;  
        int random = Random.Range(0, 2);

        if (random == 0)
        {
            localScale.x = 1; 
        }
        else
        {
            localScale.x = -1;  
        }

        transform.localScale = localScale;
    }
    public void OnCollisionEnter2D(Collision2D collider)
    {
        if (collider != null)
        {
            if (collider.gameObject.CompareTag("Guard"))
            {
                AstronautAI astroAI = collider.gameObject.GetComponent<AstronautAI>();

                if (astroAI != null)
                {
                    astroAI.stunned = true;
                   
                    Destroy(gameObject);
                }
                else
                {
                }
            }
        }
        else
        {
            return;
        }
    }
}
