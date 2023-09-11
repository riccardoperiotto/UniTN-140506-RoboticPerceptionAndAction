using Lean.Gui;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class ColoringScene_GazeableSegment : MonoBehaviour, IGazeableObject
{
    GameManager gameManager;

    // time for clicking
    private float _selectionTime = 2f;
    private SpriteRenderer _renderer;

    // managing the UI
    private Color _currentColor;
    private Color _selectionColor = Color.gray;

    void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _currentColor = _renderer.color;
    }

    public void Start()
    {
        gameManager = GameManager.Instance;
    }

    public void gazeAction()
    {
        if (gameManager.IsEyeTrackingActive)
        {
            ColoringScene_Coloring.ColorSprite(_renderer);
            _currentColor = _renderer.color;
        }
    }

    public void currentlyGazing()
    {
        if (gameManager.IsEyeTrackingActive) { _renderer.color = _selectionColor; }
    }
    public void stoppedGazing()
    {
        if (gameManager.IsEyeTrackingActive) _renderer.color = _currentColor;
    }

    public float getGazeTime()
    {
        return _selectionTime;
    }

    public void UpdateColor(Color color)
    {
        _currentColor = color;
    }
}

