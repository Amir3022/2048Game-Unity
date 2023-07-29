using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;

public class FileDataHandler
{
    private bool bUseEncryption;
    private string fileName;
    private string folderPath;
    private readonly string EncryptionWord = "Cipher";
    public FileDataHandler(string folderPath, string FileName, bool bUseEncrytion)
    {
        this.folderPath = folderPath;
        this.fileName= FileName;
        this.bUseEncryption= bUseEncrytion;
    }

    public GameData Load()
    {
        string FullPath = Path.Combine(folderPath, this.fileName);
        GameData LoadedData = null;
        if(File.Exists(FullPath))
        {
            try 
            {
                string dataToRead = "";
                using(FileStream stream = new FileStream(FullPath, FileMode.Open))
                {
                    using(StreamReader reader = new StreamReader(stream)) 
                    {
                        dataToRead = reader.ReadToEnd();
                    }
                }
                if(bUseEncryption) 
                {
                    dataToRead = EncryptDecrypt(dataToRead);
                }
                LoadedData = JsonUtility.FromJson<GameData>(dataToRead);
            }
            catch(Exception e)
            {
                Debug.Log($"Unable to load Saved game from path: {FullPath} \n{e}");
            }
        }
        return LoadedData;
    }

    public void Save(GameData gameData)
    {
        string FullPath = Path.Combine(folderPath, fileName);
        try
        {
            Directory.CreateDirectory(folderPath);
            string dataToStore = JsonUtility.ToJson(gameData, true);
            if(bUseEncryption)
            {
                dataToStore = EncryptDecrypt(dataToStore);
            }
            using(FileStream stream = new FileStream(FullPath, FileMode.Create))
            {
                using(StreamWriter writer = new StreamWriter(stream)) 
                {
                    writer.Write(dataToStore);
                }
            }
        }
        catch(Exception e)
        {
            Debug.Log($"Unable to load to file with path: {FullPath}. \n {e}");
        }
    }

    private string EncryptDecrypt(string data)
    {
        string EncryptedWord = "";
        for(int i = 0; i < data.Length; i++) 
        {
            EncryptedWord += (char)(data[i] ^ EncryptionWord[i% EncryptionWord.Length]);
        }
        return EncryptedWord;
    }
}
