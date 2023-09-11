using Lean.Gui;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class DrawingScene_WidthSelection : MonoBehaviour
{
    static public int selectedWidthIndex = 0;

    static private Color _opaqueColor = Color.white;
    static private float _combination = 0.5f;

    static public void ColorWidthSelectors()
    {
        Color color = DrawingScene_Drawing.drawable.GetColor();
        int i = 0;

        foreach (Transform width in GameObject.Find("Widths").transform)
        {
            if (i == selectedWidthIndex)
            {
                width.GetComponent<DrawingScene_GazeableWidth>().SetColor(color);
            }
            else
            {
                width.GetComponent<DrawingScene_GazeableWidth>().SetColor(Color.Lerp(_opaqueColor, color, _combination));
            }
            i++;
        }
    }


    static public void SetWidth(int index, int width)
    {
        selectedWidthIndex = index;
        DrawingScene_Drawing.drawable.SetWidth(width);
        ColorWidthSelectors();
    }


    void Start()
    {
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
                // change the width
                if (hit.collider.CompareTag("Width"))
                {
                    SetWidth(
                        hit.collider.GetComponent<DrawingScene_GazeableWidth>().associatedIndex,
                        hit.collider.GetComponent<DrawingScene_GazeableWidth>().associatedWidth
                    );
                }

            }
        }
    }




}

