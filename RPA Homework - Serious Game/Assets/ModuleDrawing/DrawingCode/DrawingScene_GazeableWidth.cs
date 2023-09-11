using Lean.Gui;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class DrawingScene_GazeableWidth : MonoBehaviour, IGazeableObject
{

    GameManager gameManager;

    public int associatedIndex;
    public int associatedWidth;

    // managing the color of the sprite
    private Color _objectColor;
    private Color _color;
    private Color _selectionColor = Color.gray;

    // time for clicking
    private float _selectionTime = 2f;
    private SpriteRenderer _renderer;

    public void SetColor(Color color)
    {
        _objectColor = color;
        _color = _objectColor;
    }
    void Awake()
    {

    }
    void Update()
    {
        _renderer.color = _color;
    }


    public void Start()
    {
        gameManager = GameManager.Instance;
        _renderer = GetComponent<SpriteRenderer>();
    }

    public void gazeAction()
    {
        if (gameManager.IsEyeTrackingActive)
        {
            DrawingScene_WidthSelection.SetWidth(
                GetComponent<DrawingScene_GazeableWidth>().associatedIndex,
                GetComponent<DrawingScene_GazeableWidth>().associatedWidth
            );
            _color = _objectColor;

        }
    }

    public void currentlyGazing()
    {
        if (gameManager.IsEyeTrackingActive) _color = _selectionColor;
    }
    public void stoppedGazing()
    {
        _color = _objectColor;
    }

    public float getGazeTime()
    {
        return _selectionTime;
    }

}

