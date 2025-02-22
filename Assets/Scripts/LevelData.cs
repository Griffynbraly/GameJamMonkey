using UnityEngine;
using System.Collections.Generic;


[CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObjects/LevelData")]
public class LevelData : ScriptableObject
{
    public Vector2 endLadderPos;
    public Vector2 buttonPos;
    public List<Vector2> DimensionsInLevel;
}

