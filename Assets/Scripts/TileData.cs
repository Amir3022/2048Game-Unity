using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct TileData
{
    [SerializeField] public Vector2 TilePos;
    [SerializeField] public int TileScore;
    public TileData(Vector2 inPos, int inScore)
    {
        TilePos = inPos;
        TileScore = inScore;
    }
    public static bool operator ==(TileData lhs, TileData rhs)
    {
        return lhs.TilePos == rhs.TilePos && lhs.TileScore == rhs.TileScore;
    }

    public static bool operator !=(TileData lhs, TileData rhs)
    {
        return !(lhs == rhs);
    }

}
