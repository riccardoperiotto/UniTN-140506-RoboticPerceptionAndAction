using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Tobii.Gaming;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    #region EyeTracker management
    private Color eyeTrackingActiveColor = new Color(8 / 255f, 148 / 255f, 247 / 255f);
    private Color eyeTrackingDeactiveColor = Color.Lerp(new Color(8 / 255f, 148 / 255f, 247 / 255f), Color.black, 0.5f);
    private Color eyeTrackingDisconnectedColor = Color.red;

    private bool isEyeTrackingActive;
    public bool IsEyeTrackingActive
    {
        get { return isEyeTrackingActive; }
        set
        {
            IsEyeTrackingConnected = true;
            isEyeTrackingActive = value && IsEyeTrackingConnected;
        }
    }

    private bool isEyeTrackingConnected;
    public bool IsEyeTrackingConnected
    {
        get { return isEyeTrackingConnected; }
        set { isEyeTrackingConnected = TobiiAPI.IsConnected && TobiiAPI.GetUserPresence().IsUserPresent(); }
    }

    public void ToggleEyeTracker()
    {
        IsEyeTrackingActive = !IsEyeTrackingActive;
        ColorEyeTrackerToggle();
    }

    public void ColorEyeTrackerToggle()
    {
        GameObject.Find("EyeTrackingToggle").transform.Find("Panel").GetComponent<Image>().color =
            // if it is active we already know it is connected
            IsEyeTrackingActive ? eyeTrackingActiveColor :
            // if it is deactive but connected then we have the same color but darker. Finally it means it is disconnected
            IsEyeTrackingConnected ? eyeTrackingDeactiveColor : eyeTrackingDisconnectedColor;
    }
    #endregion
    #region GameData management
    public GameData gameData;
    public void LoadGameData()
    {
        gameData = JsonUtility.FromJson<GameData>(File.ReadAllText($"{Application.dataPath}{GameConfig.GameDataFilePath}"));
    }

    public void SaveGameData()
    {
        File.WriteAllText($"{Application.dataPath}{GameConfig.GameDataFilePath}", JsonUtility.ToJson(gameData));
    }
    public int IncrementDrawingCounter()
    {
        gameData.DrawingCounter++;
        SaveGameData();
        return gameData.DrawingCounter;
    }
    #endregion
    #region Scene and screen management
    /// <summary>
    /// This method can be called from whatever place in the project, as the gamemanager is available everywhere.
    /// </summary>
    /// <param name="newScene"></param>
    public static void SwitchScene(string newScene)
    {
        SceneManager.LoadScene(newScene);
    }

    public static void ShiftToNext()
    {
        var shift = new Vector2(GameObject.Find("Screen 1").GetComponent<RectTransform>().rect.width, 0);
        GameObject.Find("Screens").GetComponent<RectTransform>().offsetMin -= shift;
        GameObject.Find("Screens").GetComponent<RectTransform>().offsetMax -= shift;
    }
    #endregion
    #region Monoscript
    public void Awake()
    {
        // when loading the game, always try to activate the eye tracker
        IsEyeTrackingActive = true;
        ColorEyeTrackerToggle();

        /* as suggested by the tip on it, the command DontDestroyOnLoad 
         * avoid the deletion of the GameObject to which this script is
         * attached when we change the scene
         */
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            DestroyImmediate(gameObject);
            return;
        }

        LoadGameData();
    }
    // Start is called before the first frame update
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
        ColorEyeTrackerToggle();
    }
    #endregion
}




