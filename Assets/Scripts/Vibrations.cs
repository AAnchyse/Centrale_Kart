using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Luminosity.IO;
using XInputDotNetPure;
using UnityEngine.Events;

public class Vibrations : MonoBehaviour
{
    public WheelDrive wheelDrive;
    public PlayerID playerID;
    public PlayerIndex playerIndex;
	public GamepadVibration vibration;
    public Rigidbody rb;
    public float landingForce;
    public float collisionForce;
    public float delayMax;

    [HideInInspector]
    public bool landed;

    [HideInInspector]
    public bool collision;

    private float delay;


    void Start()
    {
        vibration = new GamepadVibration();
        landed = false;
        collision = false;
        delay = 0;
    }

    void Update()
    {
        bool driftInput = InputManager.GetButton("Drift",playerID);

        if (!wheelDrive.isSkidding && wheelDrive.boostGauge >0 && !driftInput)
        {
            vibration.LeftMotor = Mathf.Clamp(wheelDrive.boostGauge, 0 ,0.05f);
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

        else if(collision)
        {
            vibration.LeftMotor = collisionForce;
		    vibration.RightMotor = vibration.LeftMotor;
            if( delay< delayMax)
                delay+= Time.deltaTime;
            else
            {
                collision = false;
                delay =0;
            }
        }
        
        else
        {
            vibration.LeftMotor = 0;
		    vibration.RightMotor = vibration.LeftMotor;
        }    

        GamePad.SetVibration(playerIndex, vibration.LeftMotor, vibration.RightMotor); //0 to 65
    }
}
