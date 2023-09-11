using Lean.Gui;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class DrawingScene_GazeableColor : MonoBehaviour, IGazeableObject
{

    GameManager gameManager;

    // managing the color of the sprite
    public Color color;
    private Color _selectionColor = Color.gray;

    // time for clicking
    private float _selectionTime = 2f;
    private SpriteRenderer _renderer;


    private bool colorInitialized = false;

    void Awake()
    {

    }

    public void Start()
    {
        gameManager = GameManager.Instance;
        _renderer = GetComponent<SpriteRenderer>();
    }

    public void Update()
    {
        if (!colorInitialized)
        {
            color = _renderer.color;
            colorInitialized = true;
        }
    }
    public void gazeAction()
    {
        if (gameManager.IsEyeTrackingActive) DrawingScene_ToolSelection.SetColor(transform, color);
    }

    public void currentlyGazing()
    {
        //if (gameManager.isEyeTrackingActive) _renderer.color = Color.Lerp(_originalColor, _selectionColor, _combination);
        if (gameManager.IsEyeTrackingActive) _renderer.color = _selectionColor;
    }

    public void stoppedGazing()
    {
        _renderer.color = color;
    }

    public float getGazeTime()
    {
        return _selectionTime;
    }

}

