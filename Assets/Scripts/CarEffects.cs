using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarEffects : MonoBehaviour
{
    public WheelDrive wheelDrive;

    public AudioSource motor;

    public float initialMotorPitch = 1;

    public GameObject backLights;

    void Update()
    {
        motor.pitch = wheelDrive.speed / wheelDrive.maxBoostSpeed +initialMotorPitch;

        if(wheelDrive.isBracking)
            backLights.SetActive(true);
        else
            backLights.SetActive(false);
            
    }
}
