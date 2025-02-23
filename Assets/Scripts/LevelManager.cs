using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    public static int level { get; private set; }
    public bool buttonPressed = false;
    private LadderButton ladderButton;
    [SerializeField] SpawnManager spawnManager;

    private List<int> basesLoaded = new List<int>();
    static public List<int> levelsMade = new List<int>();

    void Start()
    {
        levelsMade.Clear();
        basesLoaded.Clear();
        PlayerMove.OnLevelArrive += PlayerArrive;
        level = 0;
        spawnManager.LoadLevel(level);
        spawnManager.LoadLevelBase(level);
        basesLoaded.Add(level); 
    }
    private void ButtonPressed( int level)
    {
        spawnManager.SpawnEndLadder(level);
        if (!levelsMade.Contains(level))
        {
            spawnManager.LoadLevel(level + 1);
            levelsMade.Add(level);
        }
       
        if (!basesLoaded.Contains(level + 1))
        {
            spawnManager.LoadLevelBase(level + 1);
            basesLoaded.Add(level + 1);
        }
    }
    IEnumerator UnloadDelay()
    {
        yield return new WaitForSeconds(0.5f);
        spawnManager.UnloadPrevious();
    }
    private void PlayerArrive()
    {
        level += 1;
        spawnManager.DestroyEndLadder();
        spawnManager.LoadFloor(level);

        StartCoroutine(UnloadDelay());
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
        PlayerMove.OnLevelArrive -= PlayerArrive;
    }
}
