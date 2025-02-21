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

    [SerializeField] List<int> levelsMade = new List<int>();

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
            if (levelNum == 0)
            {
                Instantiate(levelBase, new Vector2(13, AdjustedY(0)), transform.rotation);
                levelsMade.Add(levelNum);
            }
        }
        //make sure the event is called last
    }
    public void SpawnEndLadder(int levelNum)
    {
        
    }

    private float AdjustedY(float y)
    {
        return y + (16 * LevelManager.level);
    }

    public void UnLoadLevel(int levelNum)
    {
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
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
