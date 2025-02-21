using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static int level { get; private set; }
    public bool buttonPressed = false;
    [SerializeField] LadderButton ladderButton;
    [SerializeField] SpawnManager spawnManager;
    void Start()
    {
        level = 0;
        spawnManager.LoadLevel(level);
        AssignButton();
        
    }

    void Update()
    {
        
    }

    private void ButtonPressed()
    {
        spawnManager.SpawnEndLadder(level);
    }

    private void AssignButton()
    {
        GameObject buttonObj = GameObject.FindGameObjectWithTag("LadderButton");
        ladderButton = (LadderButton)buttonObj.GetComponent<LadderButton>();
        ladderButton.OnButtonPress += ButtonPressed;
    }

    private void DesignButton()
    {
        ladderButton.OnButtonPress -= ButtonPressed;
    }
}
