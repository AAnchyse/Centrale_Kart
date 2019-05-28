using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Luminosity.IO;

public class Jump : MonoBehaviour
{
    public float jumpForce;
    public Rigidbody rb;

    void Update()
    {
        float jumpInput = InputManager.GetAxis("Jump");
        
        if(WheelDrive.isGrounded && jumpInput >0)
        {
            rb.AddForce(transform.InverseTransformDirection(transform.up) * jumpForce);
        }
    }
}
