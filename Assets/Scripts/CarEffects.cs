using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarEffects : MonoBehaviour
{

    public AudioSource motor;

    public float initialMotorPitch = 1;

    public GameObject backLights;

    void Update()
    {
        motor.pitch = WheelDrive.speed / WheelDrive.maxBoostSpeed +initialMotorPitch;

        if(WheelDrive.isBracking)
            backLights.SetActive(true);
        else
            backLights.SetActive(false);
            
    }
}
