using UnityEngine;
using System;

[Serializable]
public enum DriveType
{
	RearWheelDrive,
	FrontWheelDrive,
	AllWheelDrive
}

public class WheelDrive : MonoBehaviour
{
	//Public 

	[Tooltip("Rigidbody of the vehicle.")]
	public Rigidbody rb;

	[Tooltip("Position of the center of mass.")]
	public Vector3 centerOfMass;	

    [Tooltip("Maximum steering angle of the wheels.")]
	public float maxAngle = 30f;

	[Tooltip("Minimum steering angle of the wheels.")]
	public float minAngle = 5f;

	[Tooltip("Maximum torque applied to the driving wheels.")]
	public float maxTorque = 300f;

	[Tooltip("Maximum torque applied to the driving wheels when reverse drive.")]
	public float reverseMaxTorque = 300f;

	[Tooltip("Maximum torque applied to the driving wheels.")]
	public float maxBoostTorque = 300f;

	[Tooltip("Maximum vehicle's speed (in m/s).")]
	public float maxSpeed =50;

	[Tooltip("Maximum vehicle's speed (in m/s).")]
	public float maxBoostSpeed =50;

	[Tooltip("If you need the visual wheels to be attached automatically, drag the wheel shape here.")]
	public GameObject wheelMesh;

	[Tooltip("The vehicle's speed when the physics engine can use different amount of sub-steps (in m/s).")]
	public float criticalSpeed = 5f;

	[Tooltip("Simulation sub-steps when the speed is above critical.")]
	public int stepsBelow = 5;

	[Tooltip("Simulation sub-steps when the speed is below critical.")]
	public int stepsAbove = 1;

	[Tooltip("The vehicle's drive type: rear-wheels drive, front-wheels drive or all-wheels drive.")]
	public DriveType driveType;

	[Tooltip("threshold at which the boostGauge begins to fill")]
	public float slidingThreshold;

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

	[Tooltip("Indicates if we are drifting or not.")]
	private bool isDrifting;

	[Tooltip("Indicates if the 4 car's wheels are grounded.")]
	public static bool isGrounded;

	[Tooltip("Indicates if stiffness must be changed.")]
	private bool changeStiffness;

	[Tooltip("boost duration after a drift.")]
	private float boostGauge;


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
		//TODO
		//séparer ce bordel en plusieurs fonctions, voire scripts
		//enlever le get magnitude dans update, ça bouffe trop de temps
		//pb lors du saut, la voiture bouge trop, voir pour l'immobiliser
		//corriger l'atterrissage (regarder du côté du traction helper)
		//respawn la voiture quand elle est à l'envers
		
		float angleInput = Input.GetAxis("Horizontal");

		float accInput = Input.GetAxis("Vertical")*(maxSpeed - rb.velocity.magnitude) / maxSpeed;

		float driftInput = Input.GetAxis("Drift");

		float boostInput = Input.GetAxis("Boost")*(maxBoostSpeed - rb.velocity.magnitude) / maxBoostSpeed;

		handBrake = 0;

		torque = 0;

		//Acceleration
		if (Input.GetAxis("Vertical") > 0)
		{
			if((boostInput>0||boostGauge>0) /* && !isDrifting*/ )
			{
				if (boostGauge >0) boostGauge-= Time.deltaTime;

				if(rb.velocity.magnitude < maxBoostSpeed )
				{
					torque =  maxBoostTorque *(maxBoostSpeed - rb.velocity.magnitude) / maxBoostSpeed;
					handBrake = 0;
				}
				else
				{
					torque =  0;
					handBrake = maxBoostTorque;
				}
			}
			else
			{
				if(rb.velocity.magnitude < maxSpeed )
				{
					torque =  maxTorque * accInput;
					handBrake = 0;
				}
				else
				{
					torque =  0;
					handBrake = maxTorque;
				}
			}
		}
		//Deceleration
		else if (Input.GetAxis("Vertical") == 0)
		{
			torque = 0;
			handBrake = maxTorque; 
		}
		// Braking or reverse drive
		else
		{
			//Braking
			if(transform.InverseTransformDirection(rb.velocity).z> 0) 
			{
				torque = 0; 
				handBrake = Mathf.Infinity;
			}
			//Reverse drive
			else
			{
				torque = reverseMaxTorque * accInput;
				handBrake = 0;
			}
		}
		//en faire une fonction
		isGrounded = true;
		changeStiffness = false;
		float count = 0;
		foreach (WheelCollider wheel in m_Wheels)
		{
			if (!wheel.GetGroundHit(out WheelHit hit))
			{
				count +=1;
			}
			else
			{
				if((!isDrifting && driftInput >0)||(isDrifting && driftInput ==0))
				{
					changeStiffness = true;
				}
				if(isDrifting && Mathf.Abs(hit.sidewaysSlip)>slidingThreshold)
				{
					boostGauge +=Time.deltaTime/2;
				}
			}
		}
		if (count==4 )isGrounded = false;

		//Move wheels
		foreach (WheelCollider wheel in m_Wheels)
		{
			if (isGrounded)
			{
				rb.constraints = RigidbodyConstraints.None;
				// Front wheels
				if (wheel.transform.localPosition.z >= 0)
				{
					//Steering
					if(boostInput>0||boostGauge>0)
					{
						//multiplié par coeff?
						angle = angleInput * (((minAngle - maxAngle)/maxBoostSpeed) * rb.velocity.magnitude + maxAngle);
					}
					else
					{
						angle = angleInput * (((minAngle - maxAngle)/maxBoostSpeed) * rb.velocity.magnitude + maxAngle);
					}
					//Drifting
					if (driftInput>0)
					{
						if(changeStiffness)
						{
							//Friction
							ChangeFriction(wheel , stiffFrontForwardDrift , stiffFrontSidewaysDrift);
							isDrifting =true;
						}
						
						//Steering
						angle *=2; //à voir si on multplie par un coeff ou autre
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
					
					wheel.steerAngle = angle;
					
					//Torque if 4x4
					if(driveType != DriveType.RearWheelDrive)
					{
						wheel.motorTorque = torque;
					}
				}

				//Rear wheels
				if (wheel.transform.localPosition.z < 0)
				{
					//Drifting
					if (driftInput>0)
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
					if(driveType != DriveType.FrontWheelDrive)
					{
						wheel.motorTorque = torque;
					}
				}
			}
			else
			{
				rb.constraints = RigidbodyConstraints.FreezeRotationY;
			}

			// Update visual wheels if any. 
			if (wheelMesh) 
			{
				Quaternion q;
				Vector3 p;
				wheel.GetWorldPose (out p, out q);

				// Assume that the only child of the wheelcollider is the wheel shape.
				Transform shapeTransform = wheel.transform.GetChild (0);
				shapeTransform.position = p;
				shapeTransform.rotation = q;
			}
		}
	}
}