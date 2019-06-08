using System;
using UnityEngine;
using Luminosity.IO;

public class SwitchCamera : MonoBehaviour
{
    public PlayerID playerID;
    public GameObject followCam;
    public GameObject backCam;

    private bool backCamActive;
    private float camInput;

    void Start()
    {
        backCam.SetActive(false);
        followCam.SetActive(true);
        backCamActive =false;
    }

    void Update ()
    {
        float camInput = InputManager.GetAxis("Cam",playerID);

        if (camInput >0 && !backCamActive)
        {
            backCam.SetActive(true);
            followCam.SetActive(false);
            backCamActive =true;
        }
            
        if (camInput == 0 && backCamActive)
        {
            backCam.SetActive(false);
            followCam.SetActive(true);
            backCamActive =false;
        }   
    }
}
