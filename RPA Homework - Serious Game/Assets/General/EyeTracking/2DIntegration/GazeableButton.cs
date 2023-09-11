using Lean.Gui;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class GazeableButton : MonoBehaviour, IGazeableObject
{
    GameManager gameManager;

    // time for clicking
    private float _selectionTime = 2f;
    private Image _renderer;

    // managing the UI
    private Color _originalColor;
    private Color _selectionColor = Color.gray;

    void Awake()
    {
        _renderer = transform.Find("Panel").GetComponent<Image>();
        _originalColor = _renderer.color;
    }

    public void Start()
    {
        gameManager = GameManager.Instance;
    }

    public void gazeAction()
    {
        if (gameManager.IsEyeTrackingActive) GetComponent<LeanButton>().OnClick.Invoke();
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

