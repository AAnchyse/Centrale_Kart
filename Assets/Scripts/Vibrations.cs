using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Luminosity.IO;
using XInputDotNetPure;
using UnityEngine.Events;

public class Vibrations : MonoBehaviour
{
	public GamepadVibration vibration;
    public Rigidbody rb;
    public float landingForce;
    public float delayMax;

    private bool landed;
    private float delay;


    void Start()
    {
        vibration = new GamepadVibration();
        landed = false;
        delay = 0;

        WheelDrive.landedEvent.AddListener(Landed);
    }

    void Landed()
    {
        landed = true;
    }

    void Update()
    {
        bool driftInput = InputManager.GetButton("Drift");

        if (!WheelDrive.isSkidding && WheelDrive.boostGauge >0 && !driftInput)
        {
            vibration.LeftMotor = Mathf.Clamp(WheelDrive.boostGauge, 0 ,0.05f);
		    vibration.RightMotor = vibration.LeftMotor;
        }

        else if(landed)
        {
            vibration.LeftMotor = landingForce;
		    vibration.RightMotor = vibration.LeftMotor;
            if( delay< delayMax)
                delay+= Time.deltaTime;
            else
            {
                landed = false;
                delay =0;
            }
        }
        else
        {
            vibration.LeftMotor = 0;
		    vibration.RightMotor = vibration.LeftMotor;
        }    

        GamePad.SetVibration(0, vibration.LeftMotor, vibration.RightMotor); //0 to 65
    }
}
