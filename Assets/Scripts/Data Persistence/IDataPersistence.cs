using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDataPersistence
{
    void SaveData(ref GameData gameData);
    void LoadData(GameData gameData);
}
