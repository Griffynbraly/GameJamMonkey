using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour
{
    private GameObject player;
    private float speed = 10;
    private Vector3 targetPosition;
    private bool isMoving = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        PlayerMove.OnLevelArrive += OnPlayerArrive;
    }

    void Update()
    {
        if (player == null)
        {
            transform.position = new Vector3(transform.position.x, Y(), transform.position.z);
            PlayerCheck();
        }
    }

    private void OnPlayerArrive()
    {
        
        if (!isMoving)
        {
            StartCoroutine(SmoothMove());
        }
        
    }

    IEnumerator SmoothMove()
    {
        yield return new WaitForSeconds(0.005f);
        targetPosition = new Vector3(transform.position.x, Y(), transform.position.z);
        Debug.Log("Camera is moving");
        isMoving = true;
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            
            transform.position = Vector3.Lerp(transform.position, targetPosition, speed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPosition; // Ensure exact final position
        isMoving = false;
    }

    void PlayerCheck()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private int Y()
    {
        return 0 + (LevelManager.level * 16);
    }

    private void OnDisable()
    {
        PlayerMove.OnLevelArrive -= OnPlayerArrive;
    }
}
