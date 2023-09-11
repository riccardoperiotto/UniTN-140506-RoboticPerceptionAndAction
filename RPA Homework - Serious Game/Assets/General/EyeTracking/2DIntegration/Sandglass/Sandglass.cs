using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tobii.Gaming;

public class Sandglass : MonoBehaviour
{
    GameManager gameManager;

    void Start()
    {
        gameManager = GameManager.Instance;
    }
    public void Show(float gazeTime)
    {
        if (!transform.Find("RadialBar").GetComponent<RadialBar>().IsRunning())
        {
            //// Hide the gaze pointer
            GazePlotter.Plotting = false;

            // Get the point where to show it
            GazePoint gazePoint = TobiiAPI.GetGazePoint();
            Vector3 gazeOnScreen = gazePoint.Screen;
            //gazeOnScreen += new Vector3(10, 10);
            gazeOnScreen += (transform.forward * 10f);
            Vector3 gazePointInWorld = Camera.main.ScreenToWorldPoint(gazeOnScreen);

            // Restart the time
            transform.Find("RadialBar").GetComponent<RadialBar>().StartTimer(gazeTime);

            // Instanciate and display the game object
            transform.position = gazePointInWorld;
        }
    }

    public void Hide()
    {
        if (transform.Find("RadialBar").GetComponent<RadialBar>().IsRunning())
        {
            GazePlotter.Plotting = true;

            transform.position = new Vector3(0, 600);
            transform.Find("RadialBar").GetComponent<RadialBar>().StopTimer();
        }
    }

}
