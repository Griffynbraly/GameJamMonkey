using UnityEngine;
using System.Collections.Generic;
using System;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] GameObject ladder;
    [SerializeField] GameObject guard;
    [SerializeField] GameObject levelBase;
    [SerializeField] GameObject levelButton;
    [SerializeField] LevelManager levelManager;

    [SerializeField] GameObject grid;

    [SerializeField] List<int> levelsMade = new List<int>();

    [SerializeField] List<LevelData> levelDataBase = new List<LevelData>();

    static event Action OnLevelLoad;
    static event Action OnLevelUnLoad;
    void Start()
    {
        
        levelsMade.Clear();
        LoadLevel(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadLevel(int levelNum)
    {
        if (levelsMade.Contains(levelNum)) { return; }
        else
        {
            Instantiate(levelBase, new Vector2(13, AdjustedY(0)), transform.rotation);
            Instantiate(grid, new Vector2(0, AdjustedY(0)), transform.rotation);
            SpawnLadderButton(levelNum);

            levelsMade.Add(levelNum);
        }
        //make sure the event is called last
    }
    public void SpawnEndLadder(int levelNum)
    {
        LevelData levelData = levelDataBase[levelNum];
        Instantiate(ladder, levelData.endLadderPos, transform.rotation);   
    }

    public void SpawnLadderButton(int levelNum)
    {
        LevelData levelData = levelDataBase[levelNum];
        GameObject button = Instantiate(levelButton, levelData.buttonPos, transform.rotation);

        levelManager.AssignButton();
    }

    


    private float AdjustedY(float y)
    {
        return y + (16 * LevelManager.level);
    }

    public void UnLoadLevel(int levelNum)
    {
        GameObject[] allObjects = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        foreach (GameObject obj in allObjects)
        {

            if (obj.CompareTag("Player"))
            {
                break;
            }
            Destroy(obj);
        }
        //make sure the event is called last
    }
}
