using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    public Transform vehicle;
    public Checkpoint checkpoint;
    public bool respawn = false;
    public float height;

    private Transform spawn;
    private RaycastHit hit;
    private int layerMask = 1 << 8;

    void Update()
    {
        if (vehicle.position.y<height || Physics.Raycast(transform.position, transform.TransformDirection(Vector3.up), out hit, 3, layerMask) || respawn )
        {
            if(checkpoint.currentNode>=0)
                spawn = checkpoint.nodes[checkpoint.currentNode];
            else
                spawn = checkpoint.nodes[0];

            vehicle.SetPositionAndRotation(spawn.position,spawn.rotation);
            vehicle.GetComponent<Rigidbody>().velocity = new Vector3(0,0,0);
            vehicle.GetComponent<Rigidbody>().angularVelocity = new Vector3(0,0,0);
            respawn = false;
        }     
    }
}
