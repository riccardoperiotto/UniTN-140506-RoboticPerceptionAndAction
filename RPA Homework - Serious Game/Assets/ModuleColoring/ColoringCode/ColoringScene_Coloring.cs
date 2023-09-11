using Lean.Gui;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Tobii.Gaming;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ColoringScene_Coloring : MonoBehaviour
{
    static public List<Color> colorsPalette = new List<Color> {
            new Color(177.0f/255f, 50.0f/255f, 59.0f/255f),
            new Color(239/255f, 71/255f, 111/255f),
            new Color(255/255f, 209/255f, 102/255f),
            new Color(6/255f, 214/255f, 160/255f),
            new Color(17/255f, 138/255f, 178/255f),
            new Color(7/255f, 59/255f, 76/255f),
        };

    static public Transform selectedColorTransform;
    static public Color selectedColor;

    static public Vector3 normalScale = new Vector3(0.5f, 0.5f, 0.5f);
    static public Vector3 selectedScale = new Vector3(0.75f, 0.75f, 0.75f);

    static public bool paletteInitialized = false;

    static public void InitializePalette()
    {
        // fill the UI palette
        int i = 0;

        foreach (Transform color in GameObject.Find("Colors").transform)
        {
            if (i == 0)
            {
                selectedColorTransform = color.transform;
                selectedColorTransform.localScale = selectedScale;

                selectedColor = colorsPalette[0];
            }

            color.GetComponent<SpriteRenderer>().color = colorsPalette[i];
            i++;
        }

        paletteInitialized = true;
    }

    static public void SetColor(Transform transform, Color color)
    {
        selectedColorTransform.localScale = normalScale;
        selectedColorTransform = transform;
        selectedColorTransform.localScale = selectedScale;

        selectedColor = color;
    }

    static public void ColorSprite(SpriteRenderer spriteRenderer)
    {
        spriteRenderer.color = selectedColor;
    }
}

