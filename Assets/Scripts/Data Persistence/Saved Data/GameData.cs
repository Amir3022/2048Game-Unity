using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public List<TileData> CurrentTiles;
    public List<TileData> PreviousTiles;
    public int CurrentScore;

    public GameData()
    {
        CurrentTiles = new List<TileData>();
        PreviousTiles = new List<TileData>();
        CurrentScore = 0;
    }
}
