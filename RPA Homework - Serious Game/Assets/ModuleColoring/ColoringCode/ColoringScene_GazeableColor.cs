using Lean.Gui;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class ColoringScene_GazeableColor : MonoBehaviour, IGazeableObject
{
    GameManager gameManager;

    // managing the color of the sprite
    public Color color;
    public Color selectionColor = Color.gray;

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
        if (gameManager.IsEyeTrackingActive) ColoringScene_Coloring.SetColor(transform, color);
    }

    public void currentlyGazing()
    {
        if (gameManager.IsEyeTrackingActive) _renderer.color = selectionColor;
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

