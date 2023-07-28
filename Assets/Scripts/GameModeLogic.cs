using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameModeLogic : MonoBehaviour
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
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddScore(int inScore)
    {
        CurrentScore += inScore;
        LastAddedScore = inScore;
        if(ScoreBox)
        {
            ScoreBox.text = $"Score: {CurrentScore}";
        }
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
}
