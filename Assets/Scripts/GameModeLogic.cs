using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameModeLogic : MonoBehaviour, IDataPersistence
{
    private int CurrentScore;
    private int LastAddedScore;
    public Text ScoreBox;
    public GameObject GameOverScreen;
    public GameObject GameWinScreen;
    // Start is called before the first frame update

    void Start()
    {
        CurrentScore = 0;
        LastAddedScore = 0;
        StartNewGame(GameInstanceLogic.Instance.GetLoadSession(), GameInstanceLogic.Instance.GetLoadCurrent());       
    }

    public void AddScore(int inScore)
    {
        CurrentScore += inScore;
        LastAddedScore = inScore;
        UpdateScoreUI();
    }

    public void SetGameOverScreen()
    {
        if(GameOverScreen)
        {
            GameOverScreen.SetActive(true);
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SetGameWinScreen()
    {
        if(GameWinScreen)
        {
            GameWinScreen.SetActive(true);
        }
    }

    public void SaveData(ref GameData gameData)
    {
        gameData.CurrentScore= CurrentScore;
        gameData.PreviousScore = CurrentScore - LastAddedScore;
        GameInstanceLogic.Instance.SetCanRevert();
    }

    public void LoadData(GameData gameData)
    {
        CurrentScore = gameData.CurrentScore;
        LastAddedScore = CurrentScore - gameData.PreviousScore;
    }

    public void CurrentMoveOver()
    {
        DataPersistenceManager.instance.SaveGame();
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (ScoreBox)
        {
            ScoreBox.text = $"Score: {CurrentScore}";
        }
    }

    public void StartNewGame(bool bLoadGame, bool bLoadCurrent)
    {
        if (bLoadGame)
        {
            DataPersistenceManager.instance.LoadGame();
            CurrentScore = bLoadCurrent ? CurrentScore : CurrentScore - LastAddedScore;
        }
        else
            DataPersistenceManager.instance.NewGame();
        
        UpdateScoreUI();
        GridScript GridRef = GameObject.FindGameObjectWithTag("Grid Tag").GetComponent<GridScript>();
        if(GridRef != null)
        {
            GridRef.StartGameOnGrid(bLoadCurrent);
        }
        else
        {
            Debug.Log("Error, Couldn't find the GridGame Object");
        }
    }
}
