using UnityEngine;
using System;
using UnityEngine.Events;
using Luminosity.IO;

public class WheelDrive : MonoBehaviour
{
	//Public

	public PlayerID playerID;

	[Tooltip("Rigidbody of the vehicle.")]
	public Rigidbody rb;

	[Tooltip("Position of the center of mass of the rigidbody.")]
	public Vector3 centerOfMass;	

    [Tooltip("Maximum steering angle of the wheels.")]
	public float maxAngle;

	[Tooltip("Minimum steering angle of the wheels.")]
	public float minAngle;

	[Tooltip("Maximum steering angle of the wheels when boost .")]
	public float maxBoostAngle;

	[Tooltip("Minimum steering angle of the wheels when boost.")]
	public float minBoostAngle;

	[Tooltip("Maximum torque applied to the driving wheels.")]
	public float maxTorque;

	[Tooltip("Maximum torque applied to the driving wheels when reverse drive.")]
	public float reverseMaxTorque;

	[Tooltip("Maximum torque applied to the driving wheels.")]
	public float maxBoostTorque;

	[Tooltip("Maximum vehicle's speed (in m/s).")]
	public float maxSpeed;
	
	[Tooltip("Maximum vehicle's boost speed (in m/s).")]
	public float maxBoostSpeed;

	[Tooltip("If you need the visual wheels to be attached automatically, drag the wheel shape here.")]
	public GameObject wheelMesh;

	[Tooltip("The vehicle's speed when the physics engine can use different amount of sub-steps (in m/s).")]
	public float criticalSpeed;

	[Tooltip("Simulation sub-steps when the speed is above critical.")]
	public int stepsBelow;

	[Tooltip("Simulation sub-steps when the speed is below critical.")]
	public int stepsAbove;

	[Tooltip("Threshold at which the boostGauge begins to fill")]
	public float slidingThreshold;

	[Tooltip("Coefficient which multiply the steering angle when drifting")]
	public float coeffAngleSteer;

	[Tooltip("Minimum time while not grounded to get haptic feedback")]
	public float minAirTime = 1;

	[Tooltip("Indicates if the 4 car's wheels are grounded.")]
	[HideInInspector]
	public bool isGrounded;

	[Tooltip("Velocity of the rigidbody.")]
	[HideInInspector]
	public float speed;

	[Tooltip("Indicates if bracking button is pushed.")]
	[HideInInspector]
	public bool isBracking;

	[Tooltip("Indicates if we are skidding or not.")]
	[HideInInspector]
	public bool isSkidding;

	[Tooltip("Boost duration after a drift.")]
	[HideInInspector]
	public float boostGauge;

	public Vibrations vibrations;

	[Tooltip("Friction curves used to change the stiffness of the wheels.")]
	public float stiffFrontForwardNormal;
	public float stiffFrontForwardDrift;
	public float stiffFrontSidewaysNormal;
	public float stiffFrontSidewaysDrift;
	public float stiffRearForwardNormal;
	public float stiffRearForwardDrift;
	public float stiffRearSidewaysNormal;
	public float stiffRearSidewaysDrift;

	//Private

	[Tooltip("List containing the 4 Wheel colliders.")]
    private WheelCollider[] m_Wheels;

	[Tooltip("Acceleration torque applied on wheels.")]
	private float torque;

	[Tooltip("Braking torque applied on wheels.")]
	private float handBrake;

	[Tooltip("Steering angle applied on front wheels.")]
	private float angle;

	[Tooltip("Friction curves used to change the stiffness of the wheels.")]
	private WheelFrictionCurve frictionCurveForward;
	private WheelFrictionCurve frictionCurveSideways;

	[Tooltip("Indicates if stiffness must be changed.")]
	private bool changeStiffness;

	[Tooltip("Acceleration applied on wheel torque, depending of rigidbody velocity.")]
	private float acc;

	[Tooltip("Indicates if we are drifting or not.")]
	private bool isDrifting;

	[Tooltip("Time spent when not grounded. Used for haptic feedback at landing")]
	private float airTime;

	[Tooltip("Number of wheels grounded")]
	private int wheelsGrounded;
	private int previouswheelsGrounded;

	[Tooltip("List of player's inputs")]
	private float angleInput;
	private bool accInput;
	private bool brakeInput;
	private bool driftInput;
	private bool boostInput;

    // Find all the WheelColliders down in the hierarchy and instantiate wheel meshes
	void Start()
	{
		rb.centerOfMass = centerOfMass;

		m_Wheels = GetComponentsInChildren<WheelCollider>();

		m_Wheels[0].ConfigureVehicleSubsteps(criticalSpeed, stepsBelow, stepsAbove);

		for (int i = 0; i < m_Wheels.Length; ++i) 
		{
			var wheel = m_Wheels [i];

			if (wheelMesh != null)
			{
				var ws = Instantiate (wheelMesh);
				ws.transform.parent = wheel.transform;
			}
		}

		isDrifting = false;
		isBracking = false;
		airTime =0;
		wheelsGrounded =0;
		previouswheelsGrounded =0;
	}

	void ChangeFriction(WheelCollider wheel , float stiffnessForward , float stiffnessSideways)
	{
		frictionCurveForward = wheel.forwardFriction;
		frictionCurveSideways = wheel.sidewaysFriction;
		frictionCurveForward.stiffness = stiffnessForward;
		frictionCurveSideways.stiffness = stiffnessSideways;
		wheel.forwardFriction = frictionCurveForward;
		wheel.sidewaysFriction = frictionCurveSideways;
	}

	void Update()
	{	
		angleInput = InputManager.GetAxis("Turn",playerID);

		accInput = InputManager.GetButton("Acceleration",playerID);

		brakeInput = InputManager.GetButton("Brake",playerID);

		driftInput = InputManager.GetButton("Drift",playerID);

		boostInput = InputManager.GetButton("Boost",playerID);

		handBrake = 0;

		torque = 0;

		speed = rb.velocity.magnitude;

		acc = (maxSpeed - speed) / maxSpeed;

		isBracking = false;

		isSkidding =false;

		//Acceleration
		if (accInput)
		{
			// Use boost
			if(boostInput||boostGauge>0)
			{
				if (boostGauge >0) boostGauge-= Time.deltaTime;

				if(speed < maxBoostSpeed )
				{
					torque =  maxBoostTorque *(maxBoostSpeed - speed) / maxBoostSpeed;
					handBrake = 0;
				}
				else
				{
					torque =  0;
					handBrake = maxBoostTorque;
				}
			}
			// Normal drive
			else
			{
				if(speed < maxSpeed )
				{
					torque =  maxTorque * acc;
					handBrake = 0;
				}
				else
				{
					torque =  0;
					handBrake = maxTorque;
				}
			}
		}

		// Braking or reverse drive
		else if (brakeInput)
		{
			isBracking = true;
			boostGauge = 0;
			
			// Braking
			if(transform.InverseTransformDirection(rb.velocity).z> 0) 
			{
				torque = 0;
				handBrake = Mathf.Infinity;
			}
			// Reverse drive
			else
			{
				torque = -reverseMaxTorque * acc;
				handBrake = 0;
			}
		}

		//Deceleration
		else
		{
			torque = 0;
			handBrake = maxTorque;
			boostGauge = 0;
		}
			
		// Change wheel stiffness
		changeStiffness = false;
		if((!isDrifting && driftInput)||(isDrifting && !driftInput ))
		{
			changeStiffness = true;
		}

		// Check if all wheels are grounded
		isGrounded = true;
		previouswheelsGrounded = wheelsGrounded;
		wheelsGrounded = 4;
		foreach (WheelCollider wheel in m_Wheels)
		{
			if (!wheel.GetGroundHit(out WheelHit hit))
				wheelsGrounded --;

			// Update visual wheels 
			if (wheelMesh) 
			{
				Quaternion q;
				Vector3 p;
				wheel.GetWorldPose (out p, out q);

				// Assume that the only child of the wheelcollider is the wheel mesh
				Transform shapeTransform = wheel.transform.GetChild (0);
				shapeTransform.position = p;
				shapeTransform.rotation = q;
			}
		}	

		if (wheelsGrounded == 0)
		{
			isGrounded = false;
			boostGauge = 0;
		}

		//Send vibration when a new wheel hits the ground
		if(wheelsGrounded > previouswheelsGrounded)
		{
			if(airTime>minAirTime)
				vibrations.landed = true;

			//Reset airTime
			airTime = 0;
		}
	}

	void FixedUpdate()
	{
		if (wheelsGrounded<4 && wheelsGrounded>=0)
		{
			// Keep car direction while in air 
			rb.constraints = RigidbodyConstraints.FreezeRotationY;
			airTime += Time.fixedDeltaTime;	
		}

		// Remove constraints
		if (wheelsGrounded==4)
			rb.constraints = RigidbodyConstraints.None;


		// Move wheels
		foreach (WheelCollider wheel in m_Wheels)
		{	
			// Fill the boost gauge
			wheel.GetGroundHit(out WheelHit hit);
			if(isDrifting && Mathf.Abs(hit.sidewaysSlip)>slidingThreshold)
			{
				boostGauge +=Time.fixedDeltaTime/2;
				isSkidding =true;
			}

			// Front wheels
			if (wheel.transform.localPosition.z >= 0)
			{
				angle = angleInput * (((minAngle - maxAngle)/maxBoostSpeed) * speed + maxAngle);

				//Drifting
				if (driftInput)
				{
					if(changeStiffness)
					{
						//Friction
						ChangeFriction(wheel , stiffFrontForwardDrift , stiffFrontSidewaysDrift);
						isDrifting =true;
					}

					//Drift Steering
					angle *=coeffAngleSteer; 
				}
				//Normal
				else
				{
					if (changeStiffness)
					{
						//Friction
						ChangeFriction(wheel , stiffFrontForwardNormal , stiffFrontSidewaysNormal);
						isDrifting =false;
					}
				}
				//Steering angle
				wheel.steerAngle = angle;
				
				//Torque
				wheel.motorTorque = torque;
			}

			//Rear wheels
			if (wheel.transform.localPosition.z < 0)
			{
				//Drifting
				if (driftInput)
				{
					if (changeStiffness)
					{
						//Friction
						ChangeFriction(wheel , stiffRearForwardDrift , stiffRearSidewaysDrift);
						isDrifting =true;
					}
				}
				//Normal
				else
				{
					if (changeStiffness)
					{
						//Friction
						ChangeFriction(wheel , stiffRearForwardNormal , stiffRearSidewaysNormal);
						isDrifting =false;
					}
				}
				//Braking
				wheel.brakeTorque = handBrake;

				//Torque
				wheel.motorTorque = torque;
			}
		}
	}
}