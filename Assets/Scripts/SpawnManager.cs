using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] GameObject ladder;
    [SerializeField] GameObject guard;
    [SerializeField] GameObject levelBase;
    [SerializeField] GameObject levelFloor;
    [SerializeField] GameObject levelButton;
    [SerializeField] LevelManager levelManager;

    [SerializeField] GameObject objectMold;

    [SerializeField] GameObject climbMold;

    [SerializeField] GameObject grid;

    [SerializeField] GameObject topBackGround;



    [SerializeField] List<LevelData> levelDataBase = new List<LevelData>();

    private int levelToLoad;

    static event Action OnLevelLoad;
    void Start()
    {
        
        levelToLoad = 0;
        PlayerMove.OnPlayerDamaged += PlayerDamaged;
        SpawnPlayer(levelToLoad);
        LoadFloor(levelToLoad);
    }
    public void LoadLevel(int levelNum)
    {
        LoadGuards(levelNum);

        LoadObjectMolds(levelNum);
        LoadClimbMolds(levelNum);
        SpawnLadderButton(levelNum);
        if (levelNum == 10)
        {
            SpawnTopBackground();
        }
        Debug.Log($"LoadingLevel {levelNum}");
        OnLevelLoad?.Invoke();
     
        //make sure the event is called last
    }

    private void Update()
    {
    }
    public void LoadLevelBase(int levelNum)
    {
        Instantiate(levelBase, new Vector2(13, AdjustedY(0)), transform.rotation);
        Instantiate(grid, new Vector2(0, AdjustedY(0)), transform.rotation);

        Debug.Log($"LoadingBase {levelNum}");
    }
    public void LoadFloor(int levelNum)
    {
        if (levelToLoad != levelNum)
        {
            levelToLoad = levelNum;
        }
        Instantiate(levelFloor, new Vector2(0, AdjustedY(-8)), transform.rotation);
    }
    private void LoadGuards(int levelNum)
    {
        if (levelToLoad != levelNum)
        {
            levelToLoad = levelNum;
        }
        
        foreach (var pos in levelDataBase[levelNum].guardPositions)
        {
            Instantiate(guard, new Vector2(pos.x, AdjustedY(pos.y)), transform.rotation);
        }
    }

    private void LoadObjectMolds(int levelNum)
    {
        if (levelToLoad != levelNum)
        {
            levelToLoad = levelNum;
        }
        foreach (var pos in levelDataBase[levelNum].objectPositions)
        {
            GameObject instantiated = Instantiate(objectMold, new Vector2(pos.x, AdjustedY(pos.y)), transform.rotation);
            ObjectScript objectScript = instantiated.GetComponent<ObjectScript>();
            if (objectScript != null)
            {
                int index = levelDataBase[levelNum].objectPositions.IndexOf(pos);
                if (index >= 0 && index < levelDataBase[levelNum].dimensionsInLevel.Count)
                {
                    objectScript.SetScale(levelDataBase[levelNum].dimensionsInLevel[index]);
                }
            }
        }
    }
    public void PlayerDamaged()
    {
        StartCoroutine(ReloadWait());
    }

    public void SpawnTopBackground()
    {
        Instantiate(levelBase, new Vector2(13, AdjustedY(0)), transform.rotation);
        Instantiate(topBackGround, new Vector2(0, AdjustedY(0)), transform.rotation);
    }
    IEnumerator ReloadWait()
    {
        LevelManager.levelsMade.Clear();    
        yield return new WaitForSeconds(1);
        Restart();
    }
    public void SpawnPlayer(int levelNum)
    {
        if (levelToLoad != levelNum)
        {
            levelToLoad = levelNum;
        }
        Instantiate(player, new Vector2(levelDataBase[levelNum].playerSpawn.x, AdjustedY(levelDataBase[levelNum].playerSpawn.y)), transform.rotation);
    }
    public void SpawnEndLadder(int levelNum)
    {
        if (levelToLoad != levelNum)
        {
            levelToLoad = levelNum;
        }
        LevelData levelData = levelDataBase[levelNum];
        Instantiate(ladder, new Vector2(levelData.endLadderPos.x, AdjustedY(levelData.endLadderPos.y)), transform.rotation);   
    }

    private void LoadClimbMolds(int levelNum)
    {
        if (levelToLoad != levelNum)
        {
            levelToLoad = levelNum;
        }
        foreach (var pos in levelDataBase[levelNum].climbablePositions)
        {
            GameObject instantiated = Instantiate(climbMold, new Vector2(pos.x, AdjustedY(pos.y)), transform.rotation);
            ClimbableScript objectScript = instantiated.GetComponent<ClimbableScript>();
            if (objectScript != null)
            {
                int index = levelDataBase[levelNum].climbablePositions.IndexOf(pos);
                if (index >= 0 && index < levelDataBase[levelNum].climbableDimensions.Count)
                {
                    objectScript.SetScale(levelDataBase[levelNum].climbableDimensions[index]);
                }
            }
        }
    }

    public void SpawnLadderButton(int levelNum)
    {
        LevelData levelData = levelDataBase[levelNum];
        GameObject button = Instantiate(levelButton, new Vector2(levelData.buttonPos.x, AdjustedY(levelData.buttonPos.y)), transform.rotation);

        levelManager.AssignButton(button);
    }
    private float AdjustedY(float y)
    {
        return y + (16 * levelToLoad);
    }
    public void UnloadPrevious()
    {
        GameObject[] allObjects = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        foreach (GameObject obj in allObjects)
        {

            if (obj.transform.position.y < ((LevelManager.level * 16) - 10))
            {
                if (obj.CompareTag("Guard") || obj.CompareTag("Climbable") || obj.CompareTag("Walkable") || obj.CompareTag("Banana") || obj.CompareTag("LadderButton") || obj.layer == 6 || obj.CompareTag("Tile"))
                {
                    Destroy(obj);
                }
            }
        }
        //make sure the event is called last
    }

    public void Restart()
    {
        levelToLoad = LevelManager.level;
        GameObject[] allObjects = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        foreach (GameObject obj in allObjects)
        {
            if (obj.CompareTag("Guard") || obj.CompareTag("LadderButton") || obj.CompareTag("Climbable") || obj.CompareTag("Taze") || obj.CompareTag("Banana") || obj.CompareTag("Walkable"))
            {
                Destroy(obj);
            }
        }
        LoadLevel(LevelManager.level);
        SpawnPlayer(LevelManager.level);
    }
    public void DestroyEndLadder()
    {
        GameObject[] allObjects = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("EndLadder"))
            {
                Destroy(obj);
            }
        }
    }
    private void OnDisable()
    {
        PlayerMove.OnPlayerDamaged -= PlayerDamaged;
    }
}
