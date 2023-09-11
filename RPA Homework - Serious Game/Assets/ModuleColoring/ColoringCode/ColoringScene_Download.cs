using Lean.Gui;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using Tobii.Gaming;



public class ColoringScene_Download : MonoBehaviour
{
    GameManager gameManager;

    public void Download()
    {
        // DrawingScene_Drawing.drawable.drawableTexture.EncodeToPNG();
        GameObject coloringCanvas = GameObject.Find("ColoringCanvas");
        Texture2D texture = new Texture2D(600, 600,TextureFormat.RGBA32, true);
        for (int i = 0; i < texture.width; i++)
        {
            for (int j = 0; j < texture.height; j++)
            {
                texture.SetPixel(i, j, new Color(0, 0, 0, 0));
            }
        }

        foreach (Transform segment in coloringCanvas.transform)
        {
            Texture2D segmentTexture = segment.GetComponent<SpriteRenderer>().sprite.texture;
            Color color = segment.GetComponent<SpriteRenderer>().color;

            for (int i = 0; i < segmentTexture.width; i++)
            {
                for (int j = 0; j < segmentTexture.height; j++)
                {
                    if (segmentTexture.GetPixel(i, j).a > 0)
                    {
                        texture.SetPixel(i, j, color);
                    }
                }
            }

        }

        byte[] bytes = texture.EncodeToPNG();

        string dirPath = $"{Application.dataPath}{GameConfig.DownloadFolderPath}";
        if (!System.IO.Directory.Exists(dirPath))
        {
            System.IO.Directory.CreateDirectory(dirPath);
        }

        System.IO.File.WriteAllBytes($"{Application.dataPath}{GameConfig.DownloadFolderPath}/Disegno{gameManager.IncrementDrawingCounter()}.png", bytes);
    }

    public void Start()
    {
        gameManager = GameManager.Instance;
    }
}





