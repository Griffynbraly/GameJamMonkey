using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int playerHealth = 5;
    void Start()
    {
        PlayerMove.OnPlayerDamaged += PlayerDamaged;
    }

    private void PlayerDamaged()
    {
        playerHealth -= 1;
    }

    private void OnDisable()
    {
        PlayerMove.OnPlayerDamaged -= PlayerDamaged;
    }
}
