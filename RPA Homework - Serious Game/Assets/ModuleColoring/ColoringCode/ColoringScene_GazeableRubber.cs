using Lean.Gui;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class ColoringScene_GazeableRubber : MonoBehaviour, IGazeableObject
{
    GameManager gameManager;

    // time for clicking
    private float _selectionTime = 2f;
    private SpriteRenderer _renderer;

    // managing the UI
    private Color _originalColor;
    private Color _selectionColor = Color.gray;

    void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _originalColor = _renderer.color;
    }

    public void Start()
    {
        gameManager = GameManager.Instance;
    }

    public void gazeAction()
    {
        if (gameManager.IsEyeTrackingActive) ColoringScene_Coloring.SetColor(transform, Color.white);
    }

    public void currentlyGazing()
    {
        if (gameManager.IsEyeTrackingActive) _renderer.color = _selectionColor;
    }

    public void stoppedGazing()
    {
        _renderer.color = _originalColor;
    }

    public float getGazeTime()
    {
        return _selectionTime;
    }

}

