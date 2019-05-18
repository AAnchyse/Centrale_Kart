using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirControl : MonoBehaviour
{
    public Transform Front;
    public Transform Rear;
    public Transform Left;
    public Transform Right;
    public Rigidbody rb;

    public float coeff;
    void FixedUpdate()
    {
        if(! WheelDrive.isGrounded)
        {
            rb.AddRelativeTorque(-Vector3.forward*coeff*(Right.position.y-Left.position.y));
            rb.AddRelativeTorque(Vector3.right*coeff*(Front.position.y-Rear.position.y));
        }
    }
}
