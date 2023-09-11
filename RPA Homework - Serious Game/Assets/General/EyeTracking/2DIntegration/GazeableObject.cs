
/*
This is the default subclass of the GazeAware2d class. 
Drag and drop this script onto a GameObject for the GameObject to register gaze inputs.

For this script to work, the user must implement the IGazeableObject interface to the other script attached to the gameObject.
*/
using Tobii.GameIntegration.Net;
using Tobii.Gaming;
using UnityEngine;

public class GazeableObject : GazeAware2d
{
    GameManager gameManager;
    IGazeableObject gazeableObject;

    protected override void Awake()
    {
        base.Awake();
        gazeableObject = GetComponent<IGazeableObject>();
    }
    void Start()
    {
        gameManager = GameManager.Instance;
        gazeTime = gazeableObject.getGazeTime();
    }

    void OnEnable()
    {
        gazeTime = gazeableObject.getGazeTime();
    }

    protected override void gazeAction()
    {
        gazeableObject.gazeAction();
    }

    protected override void startedGazing()
    {
        if (gameManager.IsEyeTrackingActive)
        {
            gazeableObject.currentlyGazing();
            GameObject.FindWithTag("Sandglass").GetComponent<Sandglass>().Show(gazeableObject.getGazeTime());
        }
    }
    protected override void stoppedGazing()
    {
        gazeableObject.stoppedGazing();
        GameObject.FindWithTag("Sandglass").GetComponent<Sandglass>().Hide();
    }
}
