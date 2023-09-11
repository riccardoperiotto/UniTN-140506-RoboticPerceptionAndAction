using Lean.Gui;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class MemoryScene_GazeableCard : MonoBehaviour, IGazeableObject
{
    GameManager gameManager;
    GameObject gameControl;
    SpriteRenderer spriteRenderer;
    public Sprite[] faces;
    public Sprite back;
    public int faceIndex;
    public bool identicals = false;

    // time for clicking
    private float _selectionTime = 2f;
    private Image _renderer;

    // managing the UI
    private Color _originalColor;
    private Color _selectionColor = Color.gray;

    public void Awake()
    {
        gameControl = GameObject.Find("MemoryGameControl");
        spriteRenderer = transform.Find("Sprite").GetComponent<SpriteRenderer>();
        //_renderer = transform.Find("Panel").GetComponent<Image>();
        //_originalColor = _renderer.color;

       
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
        //if (gameManager.IsEyeTrackingActive) _renderer.color = _selectionColor;
    }

    public void stoppedGazing()
    {
        //_renderer.color = _originalColor;
    }

    public float getGazeTime()
    {
        return _selectionTime;
    }

    public void Flip()
    {
        //if there is no match you can flip card up and down, but only two cards' faces can be seen at once
        //if there is a match those freez  and out from the gam so the counter of card faced up restarts
        if (identicals == false)
        {
            MemoryGameControl controlScript = gameControl.GetComponent<MemoryGameControl>();
            if (spriteRenderer.sprite == back)
            {
                if (controlScript.TokenUp(this))
                {
                    spriteRenderer.sprite = faces[faceIndex];
                    controlScript.CheckTokens();
                }
            }
            else
            {
                spriteRenderer.sprite = back;
                controlScript.TokenDown(this);
            }

        }
    }

}

