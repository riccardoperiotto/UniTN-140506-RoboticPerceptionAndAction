using Lean.Gui;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using Tobii.Gaming;


/// <summary>
/// Requirements
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]

public class DrawingScene_Drawing : MonoBehaviour
{
    #region ######################### NEEDED OBJECTS ######################### 

    GameManager gameManager;

    // Used to reference THIS specific file without making all methods static
    public static DrawingScene_Drawing drawable;

    // Drawing tools options
    private Color brushColor;
    private int brushWidth = 3;

    // Behaviour when the program starts
    private bool resetCanvasOnPlay = true;
    private Color resetColor = new Color(255, 255, 255);  // By default, reset the canvas to be white

    // Unity tools
    public LayerMask drawingLayers;
    public Sprite drawableSprite;
    public Texture2D drawableTexture;

    // Objects for the drawing logic
    private Color[] cleanColorsArray;
    private Color32[] curColors;
    private bool mouseWasPreviouslyHeldDown = false;
    private bool drawOnCurrentDrag = true;
    private Vector2 previousDrawPoint; // used to draw the lines on the canvas
    private Vector2 previousGazePixelPoint; // used to calculate the velocity of the gaze movement


    // Gaze objects
    private BoxCollider2D canvasCollider;
    private float _timer = 0f;
    private float _selectionTime = 1f;


    // Distance from screen to visualization plane in the World.
    public float visualizationDistance = 10f;

    #endregion

    #region ######################### PUBLIC API ######################### 

    public void SetColor(Color newColor)
    {
        brushColor = newColor;
    }
    public Color GetColor()
    {
        return brushColor;
    }
    public void SetWidth(int newWidth)
    {
        brushWidth = newWidth;
    }
    /// <summary>
    /// Changes every pixel to be the reset color
    /// </summary>
    public void ResetCanvas()
    {
        drawable.drawableTexture.SetPixels(drawable.cleanColorsArray);
        drawable.drawableTexture.Apply();
    }

    public void Download()
    {
        byte[] bytes = drawable.drawableTexture.EncodeToPNG();
        string dirPath = $"{Application.dataPath}{GameConfig.DownloadFolderPath}";
        if (!System.IO.Directory.Exists(dirPath))
        {
            System.IO.Directory.CreateDirectory(dirPath);
        }
       ;
        System.IO.File.WriteAllBytes($"{Application.dataPath}{GameConfig.DownloadFolderPath}/Disegno{gameManager.IncrementDrawingCounter()}.png", bytes);
    }

    private void SaveTexture(Texture2D texture)
    {

    }
    #endregion

    #region ######################### COORDINATES CONVERTERS  ######################### 
    private Vector2 ProjectToPlaneInWorld(GazePoint gazePoint)
    {
        Vector3 gazeOnScreen = gazePoint.Screen;
        gazeOnScreen += (transform.forward * visualizationDistance);

        return Camera.main.ScreenToWorldPoint(gazeOnScreen);
    }

    /// <summary>
    /// Converts the world coordinates provided to be expressed as local pixel of the sprite.
    /// </summary>
    /// <param name="worldPosition"></param>
    /// <returns></returns>
    public Vector2 WorldToSpriteCoordinates(Vector2 worldPosition)
    {
        // Change coordinates to local coordinates of this image
        Vector3 localPos = transform.InverseTransformPoint(worldPosition);

        // Change these to coordinates of pixels
        float pixelWidth = drawableSprite.rect.width;
        float pixelHeight = drawableSprite.rect.height;
        float unitsToPixels = pixelWidth / drawableSprite.bounds.size.x * transform.localScale.x;

        // Need to center our coordinates
        float centeredX = localPos.x * unitsToPixels + pixelWidth / 2;
        float centeredY = localPos.y * unitsToPixels + pixelHeight / 2;

        // Round current mouse position to nearest pixel
        Vector2 pixelPos = new Vector2(Mathf.RoundToInt(centeredX), Mathf.RoundToInt(centeredY));

        return pixelPos;
    }

    #endregion

    #region ######################### DRAWING MANAGEMENT ######################### 
    /// <summary>
    /// Decide how many pixels we need to color in each direction (x and y) based on the brushWidth and the given position.
    /// </summary>
    /// <param name="centerPixel"></param>
    public void MarkPixelsToColor(Vector2 centerPixel)
    {
        int centerX = (int)centerPixel.x;
        int centerY = (int)centerPixel.y;

        for (int x = centerX - brushWidth; x <= centerX + brushWidth; x++)
        {
            // Check if the X wraps around the image, so we don't draw pixels on the other side of the image
            if (x >= (int)drawableSprite.rect.width || x < 0)
            {
                continue;
            }

            for (int y = centerY - brushWidth; y <= centerY + brushWidth; y++)
            {
                MarkPixelToChange(x, y, brushColor);
            }
        }
    }

    public void MarkPixelToChange(int x, int y, Color color)
    {
        // Need to transform x and y coordinates to flat coordinates of array
        int arrayPos = y * (int)drawableSprite.rect.width + x;


        // Check if this is a valid position
        if (arrayPos > curColors.Length || arrayPos < 0)
        {
            return;
        }

        curColors[arrayPos] = color;
    }

    /// <summary>
    /// Set the color of pixels in a straight line from startPoint all the way to endPoint, to ensure everything inbetween is colored
    /// </summary>
    /// <param name="startPoint"></param>
    /// <param name="endPoint"></param>
    /// <param name="width"></param>
    /// <param name="color"></param>
    public void ColorBetween(Vector2 startPoint, Vector2 endPoint)
    {
        // Get the distance from start to finish
        float distance = Vector2.Distance(startPoint, endPoint);

        // Calculate how many times we should interpolate between startPoint and endPoint based on the amount of time that has passed since the last update
        float lerpSteps = 1 / distance;

        for (float lerp = 0; lerp <= 1; lerp += lerpSteps)
        {
            Vector2 currentPosition = Vector2.Lerp(startPoint, endPoint, lerp);
            MarkPixelsToColor(currentPosition);
        }
    }

    public void ApplyMarkedPixelChanges()
    {
        drawableTexture.SetPixels32(curColors);
        drawableTexture.Apply();
    }

    /// <summary>
    /// Default brush type and the only one available for our project (Serious Game). 
    /// It uses only width and color. It changes the pixels surrounding the worldPoint to the static brushColor.
    /// </summary>
    /// <param name="worldPoint"></param>
    public void Draw(Vector2 pixelPos)
    {
        curColors = drawableTexture.GetPixels32();
        if (previousDrawPoint == Vector2.zero)
        {
            // If this is the first time we've ever dragged on this image, simply color the pixels at our mouse position
            MarkPixelsToColor(pixelPos);
        }
        else
        {
            // Color in a line from where we were on the last update call
            ColorBetween(previousDrawPoint, pixelPos);
        }
        ApplyMarkedPixelChanges();

        previousDrawPoint = pixelPos;
    }

    public void GazeLogic()
    {
        if (canvasCollider.bounds.IntersectRay(Camera.main.ScreenPointToRay(TobiiAPI.GetGazePoint().Screen)))
        {
            GazePoint gazePoint = TobiiAPI.GetGazePoint();
            Vector2 gazeWorldPoint = ProjectToPlaneInWorld(gazePoint);
            Vector2 gazePixelPoint = WorldToSpriteCoordinates(gazeWorldPoint);

            switch (gameManager.gameData.DrawingLogic)
            {
                case Logic.OnlyPoints:
                    Draw(gazePixelPoint);
                    previousDrawPoint = Vector2.zero;
                    break;

                case Logic.ContinuousLine:
                    Draw(gazePixelPoint);
                    break;

                case Logic.Discretized:
                    _timer += Time.deltaTime;
                    if (_timer > _selectionTime)
                    {
                        Draw(gazePixelPoint);
                        _timer = 0;
                    }
                    break;
            }
        }
        else
        {
            _timer = 0;
            previousDrawPoint = Vector2.zero;
        }
    }
    public void MouseLogic()
    {
        bool mouseHeldDown = Input.GetMouseButton(0);
        if (mouseHeldDown)
        {
            if (drawOnCurrentDrag)
            {
                // Convert mouse coordinates to world coordinates
                Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                // Check if the current mouse position overlaps our image
                Collider2D hit = Physics2D.OverlapPoint(mouseWorldPosition, drawingLayers.value);
                if (hit != null && hit.transform != null)
                {
                    // We're over the texture we're drawing on!
                    Vector2 mousePixelPos = WorldToSpriteCoordinates(mouseWorldPosition);
                    Draw(mousePixelPos);
                }
                else
                {
                    // We're not over our destination texture
                    previousDrawPoint = Vector2.zero;
                    if (!mouseWasPreviouslyHeldDown)
                    {
                        // This is a new drag where the user is left clicking off the canvas
                        // Ensure no drawing happens until a new drag is started
                        drawOnCurrentDrag = false;
                    }
                }
            }
        }
        else // Mouse is not pressed
        {
            previousDrawPoint = Vector2.zero;
            drawOnCurrentDrag = true;
        }
        mouseWasPreviouslyHeldDown = mouseHeldDown;
    }
    #endregion

    #region ######################### UNITY MONOBEHAVIOUR FUNCTIONS ######################### 

    void Awake()
    {
        // Initial objects assignment
        drawable = this;
        drawableSprite = this.GetComponent<SpriteRenderer>().sprite;
        drawableTexture = drawableSprite.texture;
        canvasCollider = this.GetComponent<BoxCollider2D>();

        // Initialize clean pixels to use
        cleanColorsArray = new Color[(int)drawableSprite.rect.width * (int)drawableSprite.rect.height];
        for (int x = 0; x < cleanColorsArray.Length; x++)
        {
            cleanColorsArray[x] = resetColor;
        }

        // Do we need to reset the canvas?
        if (resetCanvasOnPlay)
        {
            ResetCanvas();
        }
    }

    public void Start()
    {
        gameManager = GameManager.Instance;
    }

    /// <summary>
    /// This is where the magic happens.
    /// - detects wether the user is gazing or left clicking
    /// - call the appropriate function passing the drawing position
    /// </summary>
    void Update()
    {
        if (gameManager.IsEyeTrackingActive)
        {
            GazeLogic();
        }
        else
        {
            _timer = 0;
            MouseLogic();
        }
    }
    #endregion
}





