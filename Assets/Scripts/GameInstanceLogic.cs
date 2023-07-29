using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameInstanceLogic : MonoBehaviour
{
    public string GameSceneName;
    public string MainMenuSceneName;
    public static GameInstanceLogic Instance;

    private bool bLoadSession;
    private bool bLoadCurrent;
    private bool bCanRevert = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartNewGame()
    {
        Instance.bLoadSession = false;
        Instance.bLoadCurrent = true;
        SceneManager.LoadScene(GameSceneName);
    }

    public void LoadLastSession()
    {
        Instance.bLoadSession = true;
        Instance.bLoadCurrent = true;
        SceneManager.LoadScene(GameSceneName);
    }

    public void RevertToLastMove()
    {
        if (Instance.bCanRevert)
        {
            Instance.bLoadSession = true;
            Instance.bLoadCurrent = false;
            Instance.bCanRevert = false;
            SceneManager.LoadScene(GameSceneName);
        }
    }

    public void OpenMenu()
    {
        SceneManager.LoadScene(MainMenuSceneName);
    }

    public void QuitApplication()
    {
        Application.Quit();
    }

    public bool GetLoadSession()
    {
        return bLoadSession;
    }

    public bool GetLoadCurrent() 
    {
        return bLoadCurrent;
    }

    public void SetCanRevert()
    {
        bCanRevert= true;
    }
}
