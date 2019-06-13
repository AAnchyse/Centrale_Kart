using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAirControl : MonoBehaviour
{
    public AIWheelDrive wheelDrive;
    public Transform Front;
    public Transform Rear;
    public Transform Left;
    public Transform Right;
    public Rigidbody rb;

    public float coeff;
    void FixedUpdate()
    {
        if(! wheelDrive.isGrounded)
        {
            rb.AddRelativeTorque(-Vector3.forward*coeff*(Right.position.y-Left.position.y));
            rb.AddRelativeTorque(Vector3.right*coeff*(Front.position.y-Rear.position.y));
        }
    }
}
