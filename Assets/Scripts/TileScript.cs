using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class TileScript : MonoBehaviour
{
    public SpriteRenderer RendererReference;
    public float MoveSpeed = 1.0f;
    public GameObject TextObject;
    public float SpawnSpeed = 5.0f;
    public float TargetScale = 0.53f;
    private float TileSize = 2.0f;
    private Vector2 curDir;
    private bool bIsMoving = false;
    private Vector2 GridBottomLeftBorder;
    private Vector2 GridTopRightBorder;
    private GridScript GridRef;
    private int TileScore = 2;
    private Color BaseColor;
    private bool bCanConsolidate = true;
    private bool bConsolidatedATile = false;
    private Vector2 NextTileLocation;
    private bool bSpawning;
    private GameModeLogic GameModeRef;
    // Start is called before the first frame update

    void Awake()
    {
        curDir = new Vector2(0.0f, 0.0f);
        transform.localScale = new Vector3(0.0f, 0.0f, 0.0f);
        bSpawning = true;
    }

    void Start()
    {
        GameModeRef = GameObject.FindGameObjectWithTag("GameModeTag").GetComponent<GameModeLogic>();
    }

    // Update is called once per frame
    void Update()
    {
        if (bSpawning)
        {
            if (transform.localScale.x < TargetScale)
            {
                transform.localScale += Vector3.one * SpawnSpeed * Time.deltaTime;
            }
            else if (transform.localScale.y >= TargetScale)
            {
                transform.localScale = Vector3.one * TargetScale;
                bSpawning = false;
            }
        }

        if (CheckCanMove(curDir))
        {
            float DeltaX = curDir.x * MoveSpeed * Time.deltaTime;
            float DetlaY = curDir.y * MoveSpeed * Time.deltaTime;
            transform.position += new Vector3(DeltaX, DetlaY, 0.0f);
        }
        else
        {
            if (bIsMoving)
            {
                bIsMoving = false;
                curDir = Vector2.zero;
                if (GridRef)
                {
                    Vector2 NewPos = GridRef.GetNearestValidGridPosition(GetPosition2D());
                    transform.position = new Vector3(NewPos.x, NewPos.y, transform.position.z);
                }
                else
                {
                    transform.position = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), transform.position.z);
                }
            }
        }

    }

    public void MoveTile(Vector2 Dir)
    {
        bIsMoving = true;
        curDir = Dir;
    }

    public void SetBorderVariables(Vector2 BottomLeft, Vector2 TopRight)
    {
        GridBottomLeftBorder = BottomLeft;
        GridTopRightBorder = TopRight;
    }

    public bool CheckIfMoving()
    {
        return bIsMoving;
    }

    public void SetIsMoving(bool inState)
    {
        bIsMoving = inState;
    }

    private bool CheckCanMove(Vector2 Dir)
    {
        Vector3 ExpectedLocation = transform.position + new Vector3(Dir.x * MoveSpeed * Time.deltaTime, Dir.y * MoveSpeed * Time.deltaTime, 0.0f);
        bool bCanMoveNextFrame = ExpectedLocation.x >= GridBottomLeftBorder.x && ExpectedLocation.x <= GridTopRightBorder.x && ExpectedLocation.y >= GridBottomLeftBorder.y && ExpectedLocation.y <= GridTopRightBorder.y;
        if(GridRef && bCanMoveNextFrame)
        {
            Vector2 TestCollisionPoint = GetPosition2D() + new Vector2(Dir.x * (MoveSpeed * Time.deltaTime + TileSize / 2), Dir.y * (MoveSpeed * Time.deltaTime + TileSize / 2));
            TileScript NextTile = GridRef.GetTileAtInterestectingPoint(TestCollisionPoint, this);
            if(NextTile && !NextTile.CheckIfMoving())
            {
                if (TileScore == NextTile.GetTileScore() && bCanConsolidate && NextTile.CheckCanConsolidate())
                {
                    ConsolidateNextTile(NextTile);
                    bCanConsolidate = false;
                }
                else
                {
                    bCanMoveNextFrame = false;
                }
            }
            else if(bConsolidatedATile && Vector2.Distance(NextTileLocation, new Vector2(ExpectedLocation.x, ExpectedLocation.y)) <= MoveSpeed * Time.deltaTime)
            {
                bConsolidatedATile = false;
                bCanMoveNextFrame = false;
            }
        }
        return bCanMoveNextFrame && bIsMoving;
    }

    public Vector2 GetPosition2D()
    {
        return new Vector2(transform.position.x, transform.position.y);
    }

    public void SetGridRef(GridScript InGridRef)
    {
        GridRef = InGridRef;
    }

    public bool IsPointItTile(Vector2 PointVal)
    {
        Vector2 TileBottomLeft = new Vector2(transform.position.x - TileSize / 2, transform.position.y - TileSize / 2);
        Vector2 TileTopRight = new Vector2(transform.position.x + TileSize / 2, transform.position.y + TileSize / 2);
        return PointVal.x >= TileBottomLeft.x && PointVal.x <= TileTopRight.x && PointVal.y >= TileBottomLeft.y && PointVal.y <= TileTopRight.y;
    }

    public void SetColor(Color InColor)
    {
        if(RendererReference)
        {
            RendererReference.color = InColor;
        }
    }

    public void SetBaseColor(Color InColor)
    {
        BaseColor= InColor;
    }

    public void SetTileScore(int InScore)
    {
        TileScore = InScore;
        UpdateTileColor();
        if (TextObject && TextObject.GetComponent<TextMesh>())
        {
            TextObject.GetComponent<TextMesh>().text = TileScore.ToString();
        }
    }

    public int GetTileScore()
    {
        return TileScore;
    }

    private void UpdateTileColor()
    {
        if (RendererReference)
        {
            Color NewColor = BaseColor * (math.pow(0.9f, math.log2(TileScore)));
            NewColor.a = 255;
            SetColor(NewColor);
        }
    }

    private void ConsolidateNextTile(TileScript NextTile)
    {
        SetTileScore(TileScore * 2);
        bConsolidatedATile = true;
        NextTileLocation = NextTile.GetPosition2D();
        Destroy(NextTile.gameObject);
        if(GameModeRef)
        {
            GameModeRef.AddScore(TileScore);
        }
        if(GridRef)
        {
            GridRef.RemoveDestroyedTilesFromList();
            if(GameModeRef && GridRef.CheckIfAnyTileIs2048())
            {
                GameModeRef.SetGameWinScreen();
            }
        }
    }

    public bool CheckCanConsolidate()
    {
        return bCanConsolidate;
    }

    public void SetCanConsolidate(bool inState)
    {
        bCanConsolidate= inState;
    }
}
