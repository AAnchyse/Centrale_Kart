using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIWheelDrive : MonoBehaviour
{

	//Public 

	[Tooltip("Rigidbody of the vehicle.")]
	public Rigidbody rb;

	[Tooltip("Position of the center of mass of the rigidbody.")]
	public Vector3 centerOfMass;	

    [Tooltip("Maximum steering angle of the wheels.")]
	public float maxAngle;

	[Tooltip("Minimum steering angle of the wheels.")]
	public float minAngle;

	[Tooltip("Maximum torque applied to the driving wheels.")]
	public float maxTorque;

	[Tooltip("Maximum vehicle's speed (in m/s).")]
	public float maxSpeed;

	[Tooltip("If you need the visual wheels to be attached automatically, drag the wheel shape here.")]
	public GameObject wheelMesh;

	[Tooltip("The vehicle's speed when the physics engine can use different amount of sub-steps (in m/s).")]
	public float criticalSpeed;

	[Tooltip("Simulation sub-steps when the speed is above critical.")]
	public int stepsBelow;

	[Tooltip("Simulation sub-steps when the speed is below critical.")]
	public int stepsAbove;

	[Tooltip("Indicates if the 4 car's wheels are grounded.")]
	[HideInInspector]
	public bool isGrounded;

	[Tooltip("Velocity of the rigidbody.")]
	[HideInInspector]
	public float speed;

    public Checkpoint checkpoint;

    public float turnSpeed =5f;

    [Header("Sensor")]
    public float sensorLength = 3f;
    public Vector3 frontSensorPosition = new Vector3(0,0.2f,0.5f);
    public float frontSideSensorPosition = 0.2f;
    public float frontSensorAngle = 30f;

	//Private

	[Tooltip("List containing the 4 Wheel colliders.")]
    private WheelCollider[] m_Wheels;

	[Tooltip("Acceleration torque applied on wheels.")]
	private float torque;

	[Tooltip("Braking torque applied on wheels.")]
	private float handBrake;

	//[Tooltip("Steering angle applied on front wheels.")]
	//private float angle;

	[Tooltip("Acceleration applied on wheel torque, depending of rigidbody velocity.")]
	private float acc;
    

    private bool avoiding = false;
    private float targetSteerAngle =0;
    private float avoidMultiplier =0;


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
    }

    void Update()
	{	
		speed = rb.velocity.magnitude;
		acc = (maxSpeed - speed) / maxSpeed;

		//Acceleration
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

		// Check if all wheels are grounded
		isGrounded = true;
		foreach (WheelCollider wheel in m_Wheels)
		{
			if (!wheel.GetGroundHit(out WheelHit hit))
			{
				isGrounded = false;
			}

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
        if(checkpoint.finish)
        {
            torque = 0;
			handBrake = Mathf.Infinity;
        }
	}

    void FixedUpdate()
	{
        //Sensors();
        // Move wheels
        foreach (WheelCollider wheel in m_Wheels)
        {
            if (isGrounded)
            {
                // Remove constraints
                rb.constraints = RigidbodyConstraints.None;
            }
            else
            {
                // Keep car direction while in air 
                rb.constraints = RigidbodyConstraints.FreezeRotationY;
            }
            
            // Front wheels
            if (wheel.transform.localPosition.z >= 0)
            {
                //Steering angle
                if (avoiding)
                {
                    targetSteerAngle = maxAngle * avoidMultiplier;
                }
                else
                {
                    Vector3 relativeVector = transform.InverseTransformPoint(checkpoint.nodes[checkpoint.targetNode].position);
                    targetSteerAngle = (relativeVector.x /= relativeVector.magnitude) * maxAngle;
                }
                wheel.steerAngle = Mathf.Lerp(wheel.steerAngle, targetSteerAngle, Time.deltaTime* turnSpeed);
                //wheel.steerAngle = targetSteerAngle;
                //angle = angleInput * (((minAngle - maxAngle)/maxSpeed) * speed + maxAngle);
                //wheel.steerAngle = angle;
                
                //Torque
                wheel.motorTorque = torque;
            }

            //Rear wheels
            if (wheel.transform.localPosition.z < 0)
            {
                //Torque
                wheel.motorTorque = torque;
                wheel.brakeTorque = handBrake;
            }
        }
	}

    /* private void Sensors()
    {
        RaycastHit hit;
        Vector3 sensorStartPos = transform.position;
        sensorStartPos += transform.forward* frontSensorPosition.z;
        sensorStartPos += transform.up * frontSensorPosition.y;
        avoidMultiplier =0;
        avoiding= false;

        //front center sensor
        if(Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength))
        {
            if(hit.collider.gameObject.layer != LayerMask.NameToLayer("Waypoint"))
            {
                Debug.DrawLine(sensorStartPos, hit.point);
                avoiding= true;
                if(hit.normal.x <0)
                {
                    avoidMultiplier = -1;
                }
                else{
                    avoidMultiplier =1;
                }
            } 
        }
        
        //front right sensor
        sensorStartPos += transform.right * frontSideSensorPosition;
        if(Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength))
        {
            if(hit.collider.gameObject.layer != LayerMask.NameToLayer("Waypoint"))
            {
                Debug.DrawLine(sensorStartPos, hit.point);
                avoiding= true;
                avoidMultiplier-=1f;
            } 
        }
        
        //front right angle sensor
        else if(Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(frontSensorAngle, transform.up)*transform.forward, out hit, sensorLength))
        {
            if(hit.collider.gameObject.layer != LayerMask.NameToLayer("Waypoint"))
            {
                Debug.DrawLine(sensorStartPos, hit.point);
                avoiding= true;
                avoidMultiplier-=0.5f;
            } 
        }
        
        //front left sensor
        sensorStartPos -= transform.right * frontSideSensorPosition * 2;
        if(Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength))
        {
            if(hit.collider.gameObject.layer != LayerMask.NameToLayer("Waypoint"))
            {
                Debug.DrawLine(sensorStartPos, hit.point);
                avoiding= true;
                avoidMultiplier+=1f;
            } 
        }
        
        //front left angle sensor
        else if(Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(frontSensorAngle, transform.up)*transform.forward, out hit, sensorLength))
        {
            if(hit.collider.gameObject.layer != LayerMask.NameToLayer("Waypoint"))
            {
                Debug.DrawLine(sensorStartPos, hit.point);
                avoiding= true;
                avoidMultiplier+=0.5f;
            } 
        }
    }*/
}
