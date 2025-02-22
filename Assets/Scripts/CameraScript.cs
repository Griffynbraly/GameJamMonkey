using UnityEngine;

public class CameraScript : MonoBehaviour
{
    private GameObject player;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
       
    }


    void Update()
    {
        if (player == null)
        {
            transform.position = new Vector3(transform.position.x, Y(), transform.position.z);
            PlayerCheck();
        }
        else
        {
            if (player.transform.position.y >= 0)
            {
                transform.position = new Vector3(transform.position.x, player.transform.position.y, transform.position.z);
            }
        }
    }

    void PlayerCheck()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            return;
        }
    }

    private int Y()
    {
        return 0 + (LevelManager.level * 16);
    }
}
