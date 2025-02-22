using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    public static int level { get; private set; }
    public bool buttonPressed = false;
    private LadderButton ladderButton;
    [SerializeField] SpawnManager spawnManager;
    void Start()
    {
        level = 0;
        spawnManager.LoadLevel(level);
        

    }
    private void ButtonPressed( int level)
    {
        spawnManager.SpawnEndLadder(level);
        spawnManager.LoadLevel(level + 1);
    }

    public void AssignButton(GameObject button)
    {

        ladderButton = button.GetComponent<LadderButton>();
        if (ladderButton != null)
        {
            ladderButton.OnButtonPress += ButtonPressed;
        }
        else
        {
            Debug.LogWarning("LadderButton component not found on the button.");
        }

    }
    private void OnDisable()
    {
        if (ladderButton != null)
        {
            ladderButton.OnButtonPress -= ButtonPressed;
        }
    }
}
