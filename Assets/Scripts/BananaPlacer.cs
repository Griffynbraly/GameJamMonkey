using UnityEngine;

public class BananaPlacer : MonoBehaviour
{
    public bool hasBanana = false;
    [SerializeField] GameObject bananaPrefab;
    [SerializeField] Transform playerTransform;
    [SerializeField] float playerDirectionTimes;
    [SerializeField] LayerMask groundLayer; // Set this in the Inspector to detect walls
    Vector3 bananaSpawnPos;

    private void Update()
    {
        bananaSpawnPos = new Vector3(transform.position.x + (playerTransform.localScale.x * playerDirectionTimes), playerTransform.position.y, playerTransform.position.z);

        if (hasBanana && Input.GetKeyDown(KeyCode.F))
        {
            // Check for a wall before placing the banana
            if (!Physics2D.OverlapBox(bananaSpawnPos, Vector2.one, 0, groundLayer))
            {
                Instantiate(bananaPrefab, bananaSpawnPos, playerTransform.rotation);
                hasBanana = false;
            }
        }
    }

    private void OnDrawGizmos()
    {
        // Draw a box in the editor to visualize the check area
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(bananaSpawnPos, Vector3.one);
    }
}
