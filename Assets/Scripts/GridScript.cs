using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.TerrainUtils;

public struct TileData
{
    public Vector2 TilePos;
    public int TileScore;
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

public class GridScript : MonoBehaviour
{
    public GameObject TileObject;
    public Color TileInitialColor;
    private bool bCanMoveTiles = true;
    private int nRows = 4;
    private int nColumns = 4;
    private float stepSize = 1.0f;
    private float tileSize = 2.0f;
    private Vector2 BottomLeft;
    private Vector2 TopRight;
    private List<GameObject> SpawnedTiles;
    private List<Vector2> TilesPositions;
    private List<Vector2> GridPositions;
    private List<TileData> CurrentTilesData;
    private List<TileData> PreviousTilesData;
    private Vector2 MoveDir;
    private int NumberOfTiles;
    bool bTilesMoving = false;
    private GameModeLogic GameModeRef;
    // Start is called before the first frame update
    void Start()
    {
        GameModeRef = GameObject.FindGameObjectWithTag("GameModeTag").GetComponent<GameModeLogic>();
        MoveDir = new Vector2(0.0f, 0.0f);
        SpawnedTiles = new List<GameObject>();
        TilesPositions = new List<Vector2>();
        GridPositions = new List<Vector2>();
        PreviousTilesData = new List<TileData>();
        CurrentTilesData = new List<TileData>();
        Vector3 Origin = transform.position;
        BottomLeft.x = Origin.x +stepSize - (nColumns/2)*tileSize;
        BottomLeft.y = Origin.y +stepSize - (nRows/2)*tileSize;
        TopRight.x = Origin.x -stepSize + (nColumns/2)*tileSize;
        TopRight.y = Origin.y -stepSize + (nRows/2)*tileSize;
        GenerateGridPositionsList();
        SpawnTileAtLocation(Random.Range(1, nColumns-1), Random.Range(1, nRows-1));
    }

    // Update is called once per frame
    void Update()
    {
        if((Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow))&& !CheckIfAnyTileIsMoving() && bCanMoveTiles)
        {
            if(Input.GetKey(KeyCode.UpArrow))
            {
                MoveDir.x = 0.0f;
                MoveDir.y = 1.0f;
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                MoveDir.x = 0.0f;
                MoveDir.y = -1.0f;
            }
            else if(Input.GetKey(KeyCode.RightArrow))
            {
                MoveDir.x = 1.0f;
                MoveDir.y = 0.0f;
            }
            else if(Input.GetKey(KeyCode.LeftArrow))
            {
                MoveDir.x = -1.0f;
                MoveDir.y = 0.0f;
            }
            MoveTiles(MoveDir);
            bTilesMoving = true;
        }

        if(!CheckIfAnyTileIsMoving() && bTilesMoving)
        {
            bTilesMoving = false;
            FillCurrentTilesData(true);
            if (!ComparePreviousAndCurrentTilesData())
            {
                SpawnNewTileWithMove();
            }
            ResetConslidationAbilityToTiles();
            FillCurrentTilesData(false);
        }

        if (GetCurrentTilesOnGrid() >= nRows * nColumns && bCanMoveTiles)
        {
            if (!CheckForAvailableMoves())
            {
                bCanMoveTiles = false;
                if(GameModeRef)
                {
                    GameModeRef.SetGameOverScreen();
                }
            }
        }
    }

    private GameObject SpawnTileAtLocation(int _x, int _y)
    {
         if(_x < nColumns && _y < nRows)
         {
            Vector3 SpawnLoc = new Vector3(BottomLeft.x + (float)_x * tileSize, BottomLeft.y + (float)_y* tileSize, transform.position.z);
            GameObject SpawnedTile = Instantiate(TileObject, SpawnLoc, transform.rotation);
            if(SpawnedTile)
            {
                SpawnedTiles.Add(SpawnedTile);
                SpawnedTile.GetComponent<TileScript>().SetBorderVariables(BottomLeft, TopRight);
                SpawnedTile.GetComponent<TileScript>().SetGridRef(this);
                SpawnedTile.GetComponent<TileScript>().SetColor(TileInitialColor);
                int randInt = Random.Range(0, 10);
                int InitialScore = randInt % 5 == 0 ? 4 : 2;
                SpawnedTile.GetComponent<TileScript>().SetTileScore(InitialScore);

                return SpawnedTile;
            }
         }
         return null;
    }

    private void SpawnNewTileWithMove()
    {
        GetAllCurrentTilesPositions();
        if(NumberOfTiles < nRows * nColumns)
        {
            Vector2 RandomSpawnLocation;
            int xIndex;
            int yIndex;
            do
            {
                xIndex = Random.Range(0, nColumns);
                yIndex = Random.Range(0, nRows);
                RandomSpawnLocation = new Vector2(BottomLeft.x + (float)xIndex * tileSize, BottomLeft.y + (float)yIndex* tileSize);
            }while(TilesPositions.Contains(RandomSpawnLocation));
            SpawnTileAtLocation(xIndex, yIndex);
        }
        else
        {
            bCanMoveTiles= false;
        }
    }

    private void MoveTiles(Vector2 Dir)
    {
        foreach(GameObject Tile in SpawnedTiles)
        {
            if(Tile)
            {
                Tile.GetComponent<TileScript>().MoveTile(Dir);
            }
        }
    }

    private bool CheckIfAnyTileIsMoving()
    {
        foreach(GameObject Tile in SpawnedTiles)
        {
            if(Tile && Tile.GetComponent<TileScript>().CheckIfMoving())
            {
                return true;
            }
        }
        return false;
    }

    private void GetAllCurrentTilesPositions()
    {
        TilesPositions.Clear();
        NumberOfTiles = 0;
        foreach(GameObject Tile in SpawnedTiles)
        {
            if(Tile)
            {
                TilesPositions.Add(Tile.GetComponent<TileScript>().GetPosition2D());
                NumberOfTiles++;
            }
        }
    }

    public TileScript GetTileAtInterestectingPoint(Vector2 PointVal, TileScript CheckingTile)
    {
        foreach(GameObject Tile in SpawnedTiles)
        {
            if(Tile && Tile.GetComponent<TileScript>().IsPointItTile(PointVal))
            {
                if(CheckingTile)
                {
                    if(CheckingTile != Tile.GetComponent<TileScript>())
                        return Tile.GetComponent<TileScript>();
                }
                else
                    return Tile.GetComponent<TileScript>();
            }
        }
        return null;
    }

    private void GenerateGridPositionsList()
    {
        for(int y = 0; y < nRows; y++) 
        {
            for(int x = 0; x < nColumns; x++)
            {
                Vector2 NewGridPositiion = BottomLeft + new Vector2(x * tileSize,y * tileSize);
                GridPositions.Add(NewGridPositiion);
            }
        }
    }

    public Vector2 GetNearestValidGridPosition(Vector2 CurrentPosition)
    {
        float DistSqrd = -1.0f;
        int MinIndex = -1;
        for(int i = 0; i < GridPositions.Count; i++)
        {
            float CurrentDistSqrd = Vector2.SqrMagnitude(GridPositions[i] - CurrentPosition);
            if(DistSqrd < 0)
            {
                DistSqrd = CurrentDistSqrd;
                MinIndex = i;
            }
            else if(CurrentDistSqrd < DistSqrd) 
            {
                DistSqrd = CurrentDistSqrd;
                MinIndex = i;
            }
        }
        return GridPositions[MinIndex];
    }

    private void ResetConslidationAbilityToTiles()
    {
        foreach(GameObject Tile in SpawnedTiles)
        {
            if(Tile && Tile.GetComponent<TileScript>())
            {
                Tile.GetComponent<TileScript>().SetCanConsolidate(true);
            }
        }
    }

    private void FillCurrentTilesData(bool bCurrent)
    {
        if (bCurrent)
        {
            CurrentTilesData.Clear();
            foreach (GameObject Tile in SpawnedTiles)
            {
                if (Tile && Tile.GetComponent<TileScript>())
                {
                    TileData TileToAdd = new TileData(Tile.GetComponent<TileScript>().GetPosition2D(), Tile.GetComponent<TileScript>().GetTileScore());
                    CurrentTilesData.Add(TileToAdd);
                }
            }
        }
        else
        {
            PreviousTilesData.Clear();
            foreach (GameObject Tile in SpawnedTiles)
            {
                if (Tile && Tile.GetComponent<TileScript>())
                {
                    TileData TileToAdd = new TileData(Tile.GetComponent<TileScript>().GetPosition2D(), Tile.GetComponent<TileScript>().GetTileScore());
                    PreviousTilesData.Add(TileToAdd);
                }
            }
        }
    }

    private bool ComparePreviousAndCurrentTilesData()
    {
        if (CurrentTilesData.Count != PreviousTilesData.Count)
            return false;

        bool bListsAreEqual = true;
        for (int i = 0; i < CurrentTilesData.Count; i++)
        {
            if (CurrentTilesData[i] != PreviousTilesData[i])
            {
                bListsAreEqual = false;
                break;
            }
        }
        return bListsAreEqual;
    }

    private bool CheckForAvailableMoves()
    {
        bool bAvailableMove = false;
        for(int y = 0; y < nRows; y++) 
        {
            for(int x = 0; x < nColumns; x++)
            {
                TileScript CurrentTile = GetTileAtInterestectingPoint(GridPositions[x + nColumns * y], null);
                if(CurrentTile)
                {
                    if ((x - 1 + nColumns * y) >= 0 && (x - 1 + nColumns * y) < GridPositions.Count && (x - 1) >= 0)
                    {
                        TileScript LeftTile = GetTileAtInterestectingPoint(GridPositions[x - 1 + nColumns * y], null);
                        if (LeftTile && CurrentTile.GetTileScore() == LeftTile.GetTileScore())
                        {
                            bAvailableMove = true;
                            break;
                        }
                    }
                    if ((x + 1 + nColumns * y) >= 0 && (x + 1 + nColumns * y) < GridPositions.Count && (x + 1) < nColumns)
                    {
                        TileScript RightTile = GetTileAtInterestectingPoint(GridPositions[x + 1 + nColumns * y], null);
                        if (RightTile && CurrentTile.GetTileScore() == RightTile.GetTileScore())
                        {
                            bAvailableMove = true;
                            break;
                        }
                    }
                    if ((x + nColumns * (y + 1)) >= 0 && (x + nColumns * (y + 1)) < GridPositions.Count)
                    {
                        TileScript TopTile = GetTileAtInterestectingPoint(GridPositions[x + nColumns * (y + 1)], null);
                        if (TopTile && CurrentTile.GetTileScore() == TopTile.GetTileScore())
                        {
                            bAvailableMove = true;
                            break;
                        }
                    }
                    if ((x + nColumns * (y - 1)) >= 0 && (x + nColumns * (y - 1)) < GridPositions.Count)
                    {
                        TileScript BottomTile = GetTileAtInterestectingPoint(GridPositions[x + nColumns * (y - 1)], null);
                        if (BottomTile && CurrentTile.GetTileScore() == BottomTile.GetTileScore())
                        {
                            bAvailableMove = true;
                            break;
                        }
                    }
                }
            }
        }
        return bAvailableMove;
    }

    public void RemoveDestroyedTilesFromList()
    {
        SpawnedTiles.RemoveAll(Tile => Tile == null);
    }

    private int GetCurrentTilesOnGrid()
    {
        int Count = 0;
        foreach(GameObject Tile in SpawnedTiles)
        {
            if(Tile && Tile.GetComponent<TileScript>())
            {
                Count++;
            }
        }
        return Count; ;
    }

    public bool CheckIfAnyTileIs2048()
    {
        foreach(GameObject Tile in SpawnedTiles)
        {
            if(Tile && Tile.GetComponent<TileScript>())
            {
                if(Tile.GetComponent<TileScript>().GetTileScore() == 2048)
                {
                    return true;
                }
            }
        }
        return false;
    }
}
