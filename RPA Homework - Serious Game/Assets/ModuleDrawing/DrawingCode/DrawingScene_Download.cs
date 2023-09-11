using Lean.Gui;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using Tobii.Gaming;



public class DrawingScene_Download:MonoBehaviour
{
    GameManager gameManager;

    public void Download()
    {
        byte[] bytes = DrawingScene_Drawing.drawable.drawableTexture.EncodeToPNG();
        string dirPath = $"{Application.dataPath}{GameConfig.DownloadFolderPath}";
        if (!System.IO.Directory.Exists(dirPath))
        {
            System.IO.Directory.CreateDirectory(dirPath);
        }
      ;
        System.IO.File.WriteAllBytes($"{Application.dataPath}{GameConfig.DownloadFolderPath}/Disegno{gameManager.IncrementDrawingCounter()}.png", bytes);
    }

    public void Start()
    {
        gameManager = GameManager.Instance;
    }
}





