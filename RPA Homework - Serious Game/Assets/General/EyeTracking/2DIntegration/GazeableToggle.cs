using Lean.Gui;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class GazeableToggle : MonoBehaviour, IGazeableObject
{
    GameManager gameManager;

    // time for clicking
    private float _selectionTime = 2f;
    private Image _renderer;

    // managing the UI
    private Color _currentColor;
    private Color _selectedColor = new Color(8f / 255, 148f / 255, 247f / 255);
    private float _combination = 0.5f;

    void Awake()
    {
        _renderer = transform.Find("Tab").GetComponent<Image>();
    }

    public void Start()
    {
        gameManager = GameManager.Instance;
    }

    public void gazeAction()
    {
        if (gameManager.IsEyeTrackingActive)
        {
            GetComponent<LeanToggle>().TurnOn();
            _currentColor = _selectedColor;
        }
    }

    public void currentlyGazing()
    {
        if (gameManager.IsEyeTrackingActive)
        {
            _currentColor = _renderer.color;
            _renderer.color = Color.Lerp(_currentColor, _selectedColor, _combination); 
        }
    }

    public void stoppedGazing()
    {
        _renderer.color = _currentColor;
    }

    public float getGazeTime()
    {
        return _selectionTime;
    }

}

