using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public List<TileData> Tiles;
    public int CurrentScore;

    public GameData()
    {
        Tiles = new List<TileData>();
        CurrentScore = 0;
    }
}
