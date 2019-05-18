using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    public Transform vehicle;
    public Transform spawn;

    public float height;
    // Update is called once per frame
    void Update()
    {
        if (vehicle.position.y<height)
        {
            vehicle.SetPositionAndRotation(spawn.position,spawn.rotation);
            vehicle.GetComponent<Rigidbody>().velocity = new Vector3(0,0,0);
            vehicle.GetComponent<Rigidbody>().angularVelocity = new Vector3(0,0,0);
        }     
    }
}
