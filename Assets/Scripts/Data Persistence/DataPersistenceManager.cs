using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using Unity.VisualScripting.Dependencies.Sqlite;
using System;

public class DataPersistenceManager: MonoBehaviour
{
    [Header("File Storage Config")]
    [SerializeField] private bool UseEncryption;
    [SerializeField] private string Filename;

    private string SaveGamePath;

    private List<IDataPersistence> dataPersistances;
    private GameData gameData;
    private FileDataHandler dataHandler;

    public static DataPersistenceManager instance { get; private set; }

    private void Awake()
    {
        if(!instance)
        {
            Debug.Log("Only one instance of the Data Persistence manager can be created per game");
        }
        instance = this;
        SaveGamePath = Path.Combine(Application.dataPath, "SaveGames");
    }

    // Start is called before the first frame update
    void Start()
    {
        dataPersistances = GetDataPersistenceObjects();
        dataHandler = new FileDataHandler(SaveGamePath, Filename, UseEncryption);
    }

    public void NewGame()
    {
        this.gameData = new GameData();
    }

    public void LoadGame()
    {
        if(dataHandler != null)
        {
            gameData = dataHandler.Load();
            if(gameData != null) 
            {
                foreach(IDataPersistence dataPersistence in dataPersistances) 
                {
                    dataPersistence.LoadData(gameData);
                }
            }
            else
            {
                Debug.Log("Loaded data was empty, creating new game data");
                NewGame();
            }
        }
    }

    public void SaveGame()
    {
        if(dataHandler!=null) 
        {
            foreach(IDataPersistence dataPersistence in dataPersistances)
            {
                dataPersistence.SaveData(ref gameData);
            }
            dataHandler.Save(gameData);
        }
    }

    private List<IDataPersistence> GetDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistencebjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();
        return new List<IDataPersistence>(dataPersistencebjects);  
    }

    private void OnApplicationQuit()
    {
        SaveGame(); 
    }
}
