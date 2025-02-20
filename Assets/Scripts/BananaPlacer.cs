using UnityEngine;

public class BananaPlacer : MonoBehaviour
{
    private bool hasBanana = true;
    [SerializeField] GameObject bananaPrefab;
    [SerializeField] Transform playerTransform;
    Vector3 bananaSpawnPos;
    [SerializeField] float playerDirectionTimes;
    private void Update()
    {
        bananaSpawnPos = new Vector3(transform.position.x + (playerTransform.localScale.x * playerDirectionTimes), playerTransform.position.y, playerTransform.position.z);
        if (hasBanana && Input.GetKeyDown(KeyCode.F))
        {
            Instantiate(bananaPrefab, bananaSpawnPos, playerTransform.rotation);
            //hasBanana = false;
        }
    }
}
