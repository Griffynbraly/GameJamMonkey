using UnityEngine;
using System.Collections.Generic;


[CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObjects/LevelData")]
public class LevelData : ScriptableObject
{
    public Vector2 endLadderPos;
    public Vector2 buttonPos;
    public Vector2 playerSpawn;
    public List<Vector2> dimensionsInLevel;
    public List<Vector2> objectPositions;
    public List<Vector2> guardPositions;
    public List<Vector2> climbablePositions;
    public List<Vector2> climbableDimensions;
}

