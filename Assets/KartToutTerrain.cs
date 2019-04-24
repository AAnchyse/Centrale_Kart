using UnityEngine;

public class KartToutTerrain : MonoBehaviour
{

    public Rigidbody rb;
    public Vector3 centerOfMassOffset;
    public Transform[] wheelsPoints;
    public Transform wallDetector;
    public float restLength;
    public float wheelRadius;
    public LayerMask layerMask;
    public float stiffness;
    public float damping;
    public float forwardAcceleration;
    public float backwardAcceleration;
    public float deadZone;
    public float maxVelocity;
    public float maxAngularVelocity;
    public float maxBackwardVelocity;
    public float turnStrength;
    public float gravity;
    public float gravity2;
    public float groundedDrag;
    public float frictionnalFrontForce;
    public float frictionnalSideForce;
    

    private float previousLength;
    private float thrust;
    private float turnValue;
    private Vector3 normals;
    private float skid;
    private bool grounded;
    Vector3 accelerationProj;
    int counter;
    bool acceleratef;
    bool accelerateb;
    bool canSkid=true;

    // Use this for initialization
    void Start()
    {
        previousLength = restLength;

        rb =GetComponent<Rigidbody>();
        rb.centerOfMass = centerOfMassOffset;
        normals = new Vector3(0, 0, 0);
        counter = 0;
    }

    // Update is called once per frame
    void Update()
    {
        skid = 0;
        float accelerationAxis = Input.GetAxis("Vertical");

        if (accelerationAxis > deadZone)
        {
            thrust = forwardAcceleration * accelerationAxis;
            acceleratef = true;
        }
        else
            acceleratef = false;

        if (accelerationAxis < -deadZone)
        {
            thrust = backwardAcceleration * accelerationAxis;
            accelerateb = true;
        }
        else
            accelerateb = false;

        float turnAxis = Input.GetAxis("J_Horizontal");
        turnValue = turnStrength * turnAxis;

        skid = Input.GetAxis("R_Button");
        if(skid>0)
            Debug.Log("skid");

    }

    private void FixedUpdate()
    {
        //suspension Force
        int normalCounter=0;
        normals = new Vector3(0, 0, 0);
        RaycastHit hit;
        
        grounded = false;
        for (int i = 0; i < wheelsPoints.Length; i++)
        {
            if (Physics.Raycast(wheelsPoints[i].transform.position,-transform.up, out hit, restLength, layerMask))
            {
                Vector3 normalForce = hit.normal * (stiffness * (1 - (hit.distance/(restLength-wheelRadius))) + damping * (previousLength - hit.distance) / Time.fixedDeltaTime);
                rb.AddForceAtPosition(normalForce, wheelsPoints[i].transform.position - transform.up * restLength);
                grounded = true;
                normals +=hit.normal;
                normalCounter++;
            }
            else
            {
                rb.AddForce(Vector3.up * (-gravity));
                //reequilibrate the car
                if (transform.position.y <= wheelsPoints[i].transform.position.y)
                {
                    rb.AddForceAtPosition(Vector3.up * (-gravity2), wheelsPoints[i].transform.position);
                }
                else
                {
                    //LANCER UN RAYON DU TOIT DE LA BAGNOLE ET SI CA TOUCHE FONDU NOIR ET ELLE REVIENT BIEN PLACEE MERDE
                    //TODO
                    rb.AddForceAtPosition(Vector3.up * gravity, wheelsPoints[i].transform.position);
                    counter++;
                }
            }
        }
        if(normalCounter>0)
            normals /= normalCounter;

        if(counter==4)//this is the caca
        {
            for (int i = 0; i < wheelsPoints.Length; i++)
            {
                rb.AddForceAtPosition(Vector3.up * -gravity2*2, wheelsPoints[i].transform.position);
            }
        }
        counter = 0;

        //adjust drag when grounded or not
        if (grounded)
        {
            rb.drag = groundedDrag;
        }
        else
        {
            rb.drag = 0.1f;
            thrust /= 100f;
            turnValue /= 100f;
        }

        
        if (skid>0)//skid
        {
            if (Mathf.Abs(Vector3.Angle(rb.velocity, transform.forward)) < 80 &&canSkid)
            {
                accelerationProj = new Vector3(rb.velocity.x,/* Mathf.Abs(*/rb.velocity.y/*)*/, rb.velocity.z).normalized /*- (normals * (Vector3.Dot(transform.forward, normals)) / Mathf.Pow(transform.forward.magnitude, 2))*/;
                //accelerationProj *= (rb.velocity.magnitude / maxVelocity);
            }
            else
            {
                accelerationProj = new Vector3(0, 0, 0);
                canSkid = false;
            }     
        }
        else//forward acceleration
        {
            accelerationProj = transform.forward - (normals * (Vector3.Dot(transform.forward, normals)) / Mathf.Pow(transform.forward.magnitude, 2));
            canSkid = true;
        }
        
        //acceleration
        if (Mathf.Abs(thrust)>0&&(acceleratef||accelerateb))
        {
            rb.AddForce(accelerationProj * thrust);
        }
            

        //torque
        if (Mathf.Abs(turnValue) > 0 && Mathf.Abs(rb.velocity.magnitude)>5)
            rb.AddRelativeTorque(Vector3.up * turnValue);
     

        //front friction
        rb.AddForce(-transform.forward*Vector3.Dot(rb.velocity, transform.forward)*frictionnalFrontForce);
        //side friction
        rb.AddForce(-transform.right*Vector3.Dot(rb.velocity,transform.right)*frictionnalSideForce);


        // Limit max velocity
        if (rb.velocity.magnitude > (rb.velocity.normalized * maxVelocity).magnitude)
        {
            rb.velocity = rb.velocity.normalized * maxVelocity;
        }
        //Limit angular velocity
        if (rb.angularVelocity.magnitude > (rb.angularVelocity.normalized * maxAngularVelocity).magnitude)
        {
            rb.velocity = rb.velocity.normalized * maxAngularVelocity;
        }
    }
}


