using Lean.Gui;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NumbersScene_ToolSelection : MonoBehaviour
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

    static public Vector3 normalScale = new Vector3(0.5f, 0.5f, 0.5f);
    static public Vector3 selectedScale = new Vector3(0.75f, 0.75f, 0.75f);

    static public bool paletteInitialized = false;

    static public void InitializePalette()
    {
        int i = 0;

        foreach (Transform color in GameObject.Find("Colors").transform)
        {
            if (i == 0)
            {
                selectedColorTransform = color.transform;
                selectedColorTransform.localScale = selectedScale;

                NumbersScene_Drawing.drawable.SetColor(colorsPalette[0]);
            }

            color.GetComponent<SpriteRenderer>().color = colorsPalette[i];
            i++;
        }
        paletteInitialized = true;

        // only for the "Numbers" scene, default color black, but without removing all the rest
        NumbersScene_Drawing.drawable.SetColor(Color.black);

    }

    static public void SetColor(Transform transform, Color color)
    {
        selectedColorTransform.localScale = normalScale;
        selectedColorTransform = transform;
        selectedColorTransform.localScale = selectedScale;

        NumbersScene_Drawing.drawable.SetColor(color);
        NumbersScene_WidthSelection.ColorWidthSelectors();
    }

    // Start is called before the first frame update
    void Start()
    {
        InitializePalette();
        NumbersScene_WidthSelection.ColorWidthSelectors();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        Debug.DrawRay(transform.position, (Input.mousePosition), Color.green);

        if (Input.GetMouseButtonDown(0))
        {
            if (hit.collider != null)
            {
                // change the color
                if (hit.collider.CompareTag("Color"))
                {
                    SetColor(hit.collider.transform, hit.collider.GetComponent<NumbersScene_GazeableColor>().color);
                }

                // eraser
                if (hit.collider.CompareTag("Rubber"))
                {
                    SetColor(hit.collider.transform, Color.white);
                }
            }

        }


    }




}

