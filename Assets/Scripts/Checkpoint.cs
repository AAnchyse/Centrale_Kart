using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public Transform path;
    public List<Transform> nodes;
    public int currentNode = -1;
    public int targetNode = 0; 

    public Respawn respawn;
    public bool show=false;
    public int position = 0;
    public static bool updatePosition= false;
    public int round = 1;
    public float targetNodeDistance = 0; // A REGLER PLUS TARD

    void Start()
    {
        Transform[] pathTransforms = path.GetComponentsInChildren<Transform>();
        nodes = new List<Transform>();

        for(int i = 0; i< pathTransforms.Length; i++)
        {
            if(pathTransforms[i] != path.transform)
            {
                nodes.Add(pathTransforms[i]);
            }
        }
    }
    //New Waypoint
    void OnTriggerEnter(Collider collider)
    {
        updatePosition = true;
        if (collider.gameObject.layer == LayerMask.NameToLayer("Waypoint"))
        {
            int colliderNode = collider.GetComponent<NodeID>().nodeID; //currentNode
            //debug
            if(show)
            print(position);

            //we check if we haven't missed a checkpoint, otherwise we respawn at currentNode
            if(colliderNode == targetNode)
            {
                if(colliderNode ==0 && currentNode == nodes.Count - 1)
                    round++;

                currentNode = colliderNode;

                //we create a new target
                if(currentNode == nodes.Count - 1)
                {
                    targetNode = 0;
                }
                else
                {
                    targetNode = currentNode + 1;
                }
            }
            else
            {
                respawn.respawn = true;
            }
        }
    }
}
