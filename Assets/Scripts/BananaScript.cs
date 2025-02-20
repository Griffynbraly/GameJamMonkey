using UnityEngine;

public class BananaScript : MonoBehaviour
{
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
    }
}
