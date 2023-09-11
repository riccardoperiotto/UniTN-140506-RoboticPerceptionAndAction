using Lean.Gui;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Tobii.Gaming;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ColoringScene_PlayScreen : MonoBehaviour
{
    public GameObject segmentPrefab;
    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
        ColoringScene_Coloring.InitializePalette();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        if (Input.GetMouseButtonDown(0))
        {
            if (hit.collider != null)
            {
                // change the color
                if (hit.collider.CompareTag("Color"))
                {
                    ColoringScene_Coloring.SetColor(hit.collider.transform, hit.collider.GetComponent<ColoringScene_GazeableColor>().color);
                }

                // select the rubber
                if (hit.collider.CompareTag("Rubber"))
                {
                    ColoringScene_Coloring.SetColor(hit.collider.transform, Color.white);
                }

                // color the segment
                if (hit.collider.CompareTag("Segment") && hit.collider.name != "Edges")
                {
                    ColoringScene_Coloring.ColorSprite(hit.collider.GetComponent<SpriteRenderer>());
                    hit.collider.GetComponent<ColoringScene_GazeableSegment>().UpdateColor(ColoringScene_Coloring.selectedColor);
                }
            }

        }

    }


}


