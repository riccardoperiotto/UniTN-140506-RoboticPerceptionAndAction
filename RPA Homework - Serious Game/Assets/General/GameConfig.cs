using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameConfig
{
    public static string GameDataFilePath = "/StreamingAssets/Data/GameData.json";
    public static string KeyboardFilePath = "/StreamingAssets/Data/Keyboard.json";
    public static string PlayersFilePath = "/StreamingAssets/Data/Players.json";
    public static string DownloadFolderPath = "/StreamingAssets/Download";

    public static int NumberOfMathExercises = 5;

}