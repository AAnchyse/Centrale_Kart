using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCollision : MonoBehaviour
{
    public Rigidbody rb;
    private Vector3 angularVelocityNull;

    public Vibrations vibrations;
    public bool isAI;
 
    void Start()
    {
        angularVelocityNull = new Vector3(0,0,0);
    }
    
    void OnCollisionEnter(Collision collision)
    {
        rb.angularVelocity = angularVelocityNull;
        if(!isAI)
            vibrations.collision = true;
    }
    
    void OnCollisionStay(Collision collision)
    {
        rb.angularVelocity = angularVelocityNull;
    }
}
